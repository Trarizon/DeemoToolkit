using SkiaSharp;
using System.Diagnostics;
using Trarizon.Library.Collections.Extensions;
using Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;
using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Algorithm.Painting;
internal sealed partial class NotesPainter : INotesPainter, IDisposable
{
    private const byte FadeNoteAlpha = 0x3f;

    private InternalSettings _settings;

    #region INotePainter

    IReadOnlyList<Note> INotesPainter.Notes => _settings.Notes;

    float INotesPainter.MaxTime => _settings.MaxTime;

    #endregion

    public NotesPainter(IReadOnlyList<Note> notes, PaintingSettings settings)
    {
        _settings = new InternalSettings(this, notes, settings);
    }

    #region Painting Methods

    public Task<SKSurface> PaintAsync() => Task.Run(Paint);

    public SKSurface Paint()
    {
        SKSurface surface = PaintMainSurface();
        using SegmentPainters segmentPainters = PaintSegments();

        CombineSegments(surface.Canvas, segmentPainters);

        return surface;
    }

    private SKSurface PaintMainSurface(int verticalLineInterval = 0)
    {
        SKSurface surface = SKSurface.Create(_settings.ImageInfo);
        SKCanvas canvas = surface.Canvas;

        canvas.Clear(_settings.Assets.Background);

        float yTopLine = _settings.Padding.Top + _settings.TimeToPixel(_settings.SubAreaTimes.Exit);
        float yBottomLine = _settings.ImageInfo.Height - _settings.Padding.Bottom - _settings.TimeToPixel(_settings.SubAreaTimes.Exit);
        DrawHorizontalLine(canvas, _settings.Assets.TagPaint, 0, _settings.ImageInfo.Width, yTopLine);
        DrawHorizontalLine(canvas, _settings.Assets.TagPaint, 0, _settings.ImageInfo.Width, yBottomLine);

        if (verticalLineInterval > 0) {
            for (int i = 1; i < _settings.SegmentCount; i++) {
                DrawVerticalLine(canvas, _settings.Assets.TagPaint,
                    _settings.GetLeftSideXOfSegment(i * verticalLineInterval), yTopLine, yBottomLine);
            }
        }

        return surface;
    }

    private SegmentPainters PaintSegments()
    {
        var painters = new SegmentPainters(_settings);

        PaintIntervalLines(painters);

        _settings.Assets.TagPaint.Color = _settings.Assets.TagPaint.Color.WithAlpha(FadeNoteAlpha);

        PaintNotes(painters,
            paintHolds: _settings.GameVersion == GameVersion.DeemoII);

        painters.ApplySegmentMask();

        _settings.Assets.TagPaint.Color = _settings.Assets.TagPaint.Color.WithAlpha(0xff);
        PaintTexts(painters);

        return painters;
    }

    private void PaintIntervalLines(SegmentPainters painters)
    {
        float xl = _settings.LeftTagWidth;
        float xr = _settings.SegmentImageInfo.Width - _settings.RightTagWidth;
        foreach (var tag in _settings.RightTags) {
            int segId = _settings.OffsetAndGetSegmentId(tag.Time);
            DrawHorizontalLine(painters[segId].Canvas, _settings.Assets.TagPaint,
                xl, xr, _settings.OffsetAndGetY(tag.Time, segId));
        }
    }

    private void PaintNotes(SegmentPainters painters, bool paintHolds)
    {
        SegmentPainters? holdPainters = paintHolds ? new SegmentPainters(_settings, true) : null;

        // (prevId, lastId]. Range of segments containing current note, 
        (int, int) segmentRange = (_settings.SegmentCount - 2, _settings.SegmentCount - 1);
        // For hold tail
        (int, int) holdSegmentRange = segmentRange;

        foreach (Note note in _settings.Notes.ReverseROList().Where(n => n.IsVisible)) {
            PaintNote(painters, note, ref segmentRange);

            if (note.IsHold && holdPainters != null)
                PaintHold(holdPainters, note, ref holdSegmentRange);
        }

        if (holdPainters != null) {
            holdPainters.ApplySegmentMask();
            painters.CoverOn(holdPainters);
            holdPainters.Dispose();
        }
    }

    private void PaintNote(SegmentPainters painters, Note note, ref (int Prev, int Last) segments)
    {
        float paintTime = _settings.Offset(note.Time);

        // Adjust prevId
        while (segments.Prev >= 0 && paintTime <= _settings.SegmentEndTime(segments.Prev))
            segments.Prev--;

        for (int id = segments.Last; id > segments.Prev; id--) {
            // Adjust to-be-draw segments
            if (segments.Last >= 1 && paintTime <= _settings.SegmentStartTime(id))
                segments.Last--;
            // Find position and draw
            else if (paintTime <= _settings.SegmentMainAreaStartTime(id))
                DrawNote(painters, id, note, _settings.Assets.TagPaint);
            else if (paintTime <= _settings.SegmentMainAreaEndTime(id))
                DrawNote(painters, id, note, null);
            else if (paintTime <= _settings.SegmentEndTime(id))
                DrawNote(painters, id, note, _settings.Assets.TagPaint);
            else
                Debug.Assert(false, "Unexcepted condition.");
        }
    }

    private void PaintHold(SegmentPainters painters, Note note, ref (int Prev, int Last) segments)
    {
        float paintTime = _settings.Offset(note.Time);
        float paintTimeEnd = _settings.Offset(note.EndTime);

        while (segments.Prev >= 0 && paintTime <= _settings.SegmentEndTime(segments.Prev))
            segments.Prev--;

        for (int id = segments.Last; id > segments.Prev; id--) {
            // Adjust to-be-draw segments
            if (segments.Last >= 1 && paintTimeEnd <= _settings.SegmentStartTime(id))
                segments.Last--;
            // Draw
            else
                DrawHoldNote(painters, id, note);
        }
    }

    private void PaintTexts(SegmentPainters painters)
    {
        float halfText = _settings.Assets.TagPaint.TextSize / 2f;

        // Left
        _settings.Assets.TagPaint.TextAlign = SKTextAlign.Right;
        foreach (var tag in _settings.LeftTags) {
            DrawText(painters, tag, _settings.LeftTagWidth);
        }

        // Right
        _settings.Assets.TagPaint.TextAlign = SKTextAlign.Left;
        foreach (var tag in _settings.RightTags) {
            DrawText(painters, tag, _settings.SegmentImageInfo.Width - _settings.RightTagWidth);
        }

        void DrawText(in SegmentPainters painters, in PaintingIntervalTag tag, float x)
        {
            float offsetTime = _settings.Offset(tag.Time);
            if (offsetTime < 0f)
                return;

            int id = _settings.GetSegmentId(offsetTime);
            painters[id].Canvas.DrawText(tag.Text,
                x, _settings.GetY(offsetTime, id) + halfText, // Skiasharp text is aligned to bottom left
                _settings.Assets.TagPaint);
        }
    }

    private void CombineSegments(SKCanvas canvas, params SegmentPainters[] multiPainters)
    {
        for (int i = 0; i < multiPainters.Length; i++) {
            for (int j = 0; j < _settings.SegmentCount; j++) {
                multiPainters[i][j].Surface.Draw(canvas,
                    _settings.GetLeftSideXOfSegment(i + j * multiPainters.Length),
                    _settings.Padding.Top - _settings.Assets.OneSidePreservedBlank, null);
            }
        }
    }

    private void DrawNote(SegmentPainters painters, int segmentId, Note note, SKPaint? paint)
    {
        SKBitmap noteImg = note switch { { IsSwipe: true } => _settings.Assets.SwipeNote ?? _settings.Assets.PianoNote, { IsSlide: true } => _settings.Assets.SlideNote, { HasSound: true } => _settings.Assets.PianoNote,
            _ => _settings.Assets.NosoundNote ?? _settings.Assets.PianoNote,
        };

        float centerX = _settings.SegmentHorizontalCenter + note.Position * InternalSettings.NoteWidth1x;
        float centerY = _settings.OffsetAndGetY(note.Time, segmentId);
        float halfX = noteImg.Width / 2 * note.Size;
        float halfY = noteImg.Height / 2;
        painters[segmentId].Canvas.DrawBitmap(noteImg, new SKRect(
            left: centerX - halfX,
            top: centerY - halfY,
            right: centerX + halfX,
            bottom: centerY + halfY), paint);
    }

    private void DrawHoldNote(SegmentPainters painters, int segmentId, Note note)
    {
        if (!note.IsHold || _settings.Assets.HoldNote == null) return;
        SKBitmap noteImg = _settings.Assets.HoldNote;

        float centerX = _settings.SegmentHorizontalCenter + note.Position * InternalSettings.NoteWidth1x;
        float y = _settings.OffsetAndGetY(note.EndTime, segmentId);
        float halfX = noteImg.Width / 2 * note.Size;
        float height = _settings.TimeToPixel(note.Duration);
        painters[segmentId].Canvas.DrawBitmap(noteImg, new SKRect(
            left: centerX - halfX,
            top: y,
            right: centerX + halfX,
            bottom: y + height));
    }

    private static void DrawHorizontalLine(SKCanvas canvas, SKPaint paint, float x0, float x1, float y)
       => canvas.DrawLine(x0, y, x1, y, paint);

    private static void DrawVerticalLine(SKCanvas canvas, SKPaint paint, float x, float y0, float y1)
        => canvas.DrawLine(x, y0, x, y1, paint);

    #endregion

    public void Dispose()
    {
        _settings.Dispose();
    }

    #region Internal Types

    private class SegmentPainters : IDisposable
    {
        private readonly SKSurface _horizontalFadeMask;
        private readonly SegmentPainter[] _painters;

        public SegmentPainter this[int index] => _painters[index];

        public SegmentPainters(InternalSettings settings, bool isForHold = false)
        {
            _painters = new SegmentPainter[settings.SegmentCount];

            for (int i = 0; i < settings.SegmentCount; i++)
                _painters[i] = new SegmentPainter(settings);

            _horizontalFadeMask = SKSurface.Create(settings.SegmentImageInfo);

            using var maskPaint = isForHold ? GetHoldMask(settings) : GetNotesMask(settings);
            _horizontalFadeMask.Canvas.DrawRect(0, 0,
                settings.SegmentImageInfo.Width, settings.SegmentImageInfo.Height, maskPaint);
        }

        private static readonly float[] HoldMaskColorPosition = [0f, 0f, 1f, 1f,];
        private static SKPaint GetHoldMask(InternalSettings settings) => new() {
            Shader = SKShader.CreateLinearGradient(
            start: new SKPoint(0, settings.Assets.OneSidePreservedBlank + settings.TimeToPixel(settings.SubAreaTimes.Exit)),
            end: new SKPoint(0, settings.SegmentImageInfo.Height - settings.Assets.OneSidePreservedBlank - settings.TimeToPixel(settings.SubAreaTimes.Enter)),
            colors: [
                new(0xffffff | (FadeNoteAlpha << 24)),
                SKColors.White,
                SKColors.White,
                new(0xffffff | (FadeNoteAlpha << 24)),
            ],
            HoldMaskColorPosition,
            SKShaderTileMode.Clamp),
        };

        private static readonly SKColor[] NoteMaskColors = [SKColors.Transparent, SKColors.White, SKColors.White, SKColors.Transparent,];
        private static readonly float[] NoteMaskColorPosition = [0, InternalSettings.XFadeInPercent, 1 - InternalSettings.XFadeInPercent, 1,];
        private static SKPaint GetNotesMask(InternalSettings settings) => new() {
            Shader = SKShader.CreateLinearGradient(
                start: new SKPoint(settings.LeftTagWidth, 0),
                end: new SKPoint(settings.SegmentImageInfo.Width - settings.RightTagWidth, 0),
                NoteMaskColors,
                NoteMaskColorPosition,
                SKShaderTileMode.Clamp),
        };

        public void CoverOn(in SegmentPainters other)
        {
            for (int i = 0; i < _painters.Length; i++) {
                _painters[i].Surface.Draw(other[i].Canvas, 0, 0, null);
                _painters[i].Dispose();
                _painters[i] = other[i];
                other._painters[i] = default;
            }
        }

        public void ApplySegmentMask()
        {
            using var paint = new SKPaint { BlendMode = SKBlendMode.DstIn };
            foreach (var painter in _painters)
                _horizontalFadeMask.Draw(painter.Canvas, 0, 0, paint);
        }

        public void Dispose()
        {
            _horizontalFadeMask.Dispose();
            foreach (var painter in _painters)
                painter.Dispose();
        }
    }

    private readonly struct SegmentPainter : IDisposable
    {
        public readonly SKSurface Surface;
        public readonly SKCanvas Canvas;

        public SegmentPainter(InternalSettings settings)
        {
            Surface = SKSurface.Create(settings.SegmentImageInfo);
            Canvas = Surface.Canvas;
        }

        public void Dispose() => Surface?.Dispose();
    }

    #endregion
}
