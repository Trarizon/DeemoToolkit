using SkiaSharp;
using Trarizon.Toolkit.Deemo.Assets;
using Trarizon.Toolkit.Deemo.Commands.Utility;

namespace Trarizon.Toolkit.Deemo.Commands.Components.Painting;
public sealed class PaintingAssets : IDisposable
{
    private readonly InternalAssets _internal;

    public int OneSidePreservedBlank { get; }

    public SKColor Background => _internal.Background;
    public SKPaint TagPaint { get; }

    public SKBitmap PianoNote => _internal.PianoNote;
    public SKBitmap SlideNote => _internal.SlideNote;
    public SKBitmap? NosoundNote => _internal.NosoundNote;
    public SKBitmap? SwipeNote => _internal.SwipeNote;
    public SKBitmap? HoldNote => _internal.HoldNote;

    public PaintingAssets(GameVersion gameVersion)
    {
        _internal = gameVersion switch {
            GameVersion.Deemo => InternalAssets.Deemo,
            GameVersion.DeemoReborn => throw new NotImplementedException(),
            GameVersion.DeemoII => InternalAssets.DeemoII,
            _ => throw new NotImplementedException(),
        };
        TagPaint = _internal.TagPaint.Clone();
        OneSidePreservedBlank = (MathUtil.Max<int>(stackalloc int[] {
            PianoNote.Height,
            SlideNote.Height,
            NosoundNote?.Height ?? 0,
            SwipeNote?.Height ?? 0
        }) + 1) / 2; // Round up
    }

    public void Dispose()
    {
        TagPaint.Dispose();
    }

    private class InternalAssets : IDisposable
    {
        public static InternalAssets Deemo = new() {
            Background = SKColors.Black,
            TagPaint = new SKPaint { Color = SKColors.White, IsAntialias = true },
            PianoNote = SKBitmap.Decode(LocalAssets.DeemoPianoNoteImageFile),
            SlideNote = SKBitmap.Decode(LocalAssets.DeemoSlideNoteImageFile),
            NosoundNote = SKBitmap.Decode(LocalAssets.DeemoNosoundNoteImageFile),
        };

        public static InternalAssets DeemoII = new() {
            Background = SKColors.Black,
            TagPaint = new SKPaint { Color = SKColors.White, IsAntialias = true },
            PianoNote = SKBitmap.Decode(LocalAssets.DeemoPianoNoteImageFile),
            SlideNote = SKBitmap.Decode(LocalAssets.DeemoSlideNoteImageFile),
            NosoundNote = SKBitmap.Decode(LocalAssets.DeemoNosoundNoteImageFile),
            HoldNote = Hold(),
        }; // TODO: DeemoII Assets

        private static SKBitmap Hold()
        {
            var sur = SKSurface.Create(new SKImageInfo(100, 100));
            sur.Canvas.Clear(SKColors.White);
            var strea = sur.Snapshot().Encode().AsStream();
            return SKBitmap.Decode(strea);
        }

        public SKColor Background { get; private init; }
        public SKPaint TagPaint { get; private init; }

        public SKBitmap PianoNote { get; private init; }
        public SKBitmap SlideNote { get; private init; }
        public SKBitmap? NosoundNote { get; private init; }
        public SKBitmap? SwipeNote { get; private init; }
        public SKBitmap? HoldNote { get; private init; }

#pragma warning disable CS8618
        private InternalAssets() { }
#pragma warning restore CS8618

        public void Dispose()
        {
            TagPaint.Dispose();
            PianoNote.Dispose();
            SlideNote.Dispose();
            NosoundNote?.Dispose();
            SwipeNote?.Dispose();
            HoldNote?.Dispose();
        }
    }
}
