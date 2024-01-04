using CommunityToolkit.Diagnostics;
using SkiaSharp;
using Trarizon.Toolkit.Deemo.Algorithm.Painting.Interval;
using Trarizon.Toolkit.Deemo.Algorithm.Utilities;
using Trarizon.Toolkit.Deemo.ChartModels;

namespace Trarizon.Toolkit.Deemo.Algorithm.Painting;
internal partial class NotesPainter
{
    private sealed class InternalSettings : IDisposable
    {
        public const int NoteWidth1x = 100;
        public const int SegmentWidth = NoteWidth1x * 6;
        public const float XFadeInPercent = NoteWidth1x * 0.1f / SegmentWidth;
        public const int SpeedBase = 200;

        private readonly NotesPainter _painter;

        public readonly IReadOnlyList<Note> Notes;

        // Global parameters
        public readonly GameVersion GameVersion;
        public readonly bool ShowMidi; // TODO: show midi on background
        public readonly int PixelPerSecond;
        public readonly PaintingAssets Assets;
        private readonly float _timeOffset;
        public readonly float MaxTime;

        // Segment canvas settings
        public readonly float SegmentMainAreaTime;
        public readonly (float Enter, float Exit) SubAreaTimes;
        public readonly int SegmentCount;
        private readonly IPaintingInterval _leftInterval;
        private readonly IPaintingInterval _rightInterval;
        public IEnumerable<PaintingIntervalTag> LeftTags => _leftInterval.GetTags(_painter);
        public IEnumerable<PaintingIntervalTag> RightTags => _rightInterval.GetTags(_painter);
        public readonly float LeftTagWidth;
        public readonly float RightTagWidth;
        public readonly SKImageInfo SegmentImageInfo;

        // Global canvas settings
        public readonly (int Left, int Right, int Top, int Bottom) Padding;
        public readonly int GapWidth;
        public readonly SKImageInfo ImageInfo;

        // Calculated
        public int SegmentHorizontalCenter => (int)LeftTagWidth + SegmentWidth / 2;

        public InternalSettings(NotesPainter painter, IReadOnlyList<Note> notes, PaintingSettings settings)
        {
            _painter = painter;
            Notes = notes;

            GameVersion = settings.GameVersion;
            ShowMidi = settings.ShowMidi;
            PixelPerSecond = (int)settings.Speed * SpeedBase;
            Assets = new PaintingAssets(GameVersion);
            _timeOffset = (settings.SkipLeadingBlank, ShowMidi) switch {
                (false, _) => 0f,
                (true, false) => Notes.FirstOrDefault(n => n.IsVisible)?.Time ?? ThrowHelper.ThrowInvalidOperationException<float>("No valid note"),
                (true, true) => Notes.Count > 0 ? Notes[0].Time : ThrowHelper.ThrowInvalidOperationException<float>("No valid note"),
            };
            MaxTime = ShowMidi
                ? Notes[^1].EndTime
                : Notes.LastOrDefault(n => n.IsVisible)?.EndTime ?? 0f;

            float offsetMaxTime = Offset(MaxTime);
            SegmentMainAreaTime = float.Min(offsetMaxTime, settings.SegmentMainAreaTimeSeconds);
            SubAreaTimes = settings.SubAreaTimes;
            SegmentCount = GetSegmentId(offsetMaxTime) + 1;
            _leftInterval = settings.LeftInterval?.Clone() ?? NoInterval.Instance;
            _rightInterval = settings.RightInterval?.Clone() ?? NoInterval.Instance;
            LeftTagWidth = _leftInterval.GetTextWidth(Assets.TagPaint);
            RightTagWidth = _rightInterval.GetTextWidth(Assets.TagPaint);
            SegmentImageInfo = new SKImageInfo(
                width: SegmentWidth + (int)(LeftTagWidth + RightTagWidth),
                height: (int)TimeToPixel(SegmentMainAreaTime + SubAreaTimes.Sum()) + (Assets.OneSidePreservedBlank << 1));

            Padding = settings.Padding;
            GapWidth = settings.SegmentsGapWidth;
            ImageInfo = new SKImageInfo(
                width: Padding.Left + Padding.Right + SegmentCount * (SegmentImageInfo.Width + GapWidth) - GapWidth,
                height: Padding.Top + Padding.Bottom + SegmentImageInfo.Height - (Assets.OneSidePreservedBlank << 1));
        }

        public float PixelToTime(int pixel) => (float)pixel / PixelPerSecond;
        public float TimeToPixel(float time) => time * PixelPerSecond;

        public float Offset(float originalTime) => originalTime - _timeOffset;

        public int GetSegmentId(float timeAfterOffset) => timeAfterOffset switch {
            0f => 0,
            < 0f => ThrowHelper.ThrowArgumentOutOfRangeException<int>(nameof(timeAfterOffset), message: "time < 0f."),
            _ => (int)MathF.Ceiling(timeAfterOffset / SegmentMainAreaTime) - 1,
        };
        public int OffsetAndGetSegmentId(float time) => GetSegmentId(Offset(time));

        public float SegmentMainAreaStartTime(int segmentId) => segmentId * SegmentMainAreaTime;
        public float SegmentMainAreaEndTime(int segmentId) => SegmentMainAreaStartTime(segmentId + 1);
        public float SegmentStartTime(int segmentId) => SegmentMainAreaStartTime(segmentId) - SubAreaTimes.Enter;
        public float SegmentEndTime(int segmentId) => SegmentMainAreaEndTime(segmentId) + SubAreaTimes.Exit;

        private float GetRelativeTimeToSegment(float timeAfterOffset, int segmentId)
            => timeAfterOffset - SegmentMainAreaStartTime(segmentId);
        public float GetY(float timeAfterOffset, int segmentId)
            => SegmentImageInfo.Height - Assets.OneSidePreservedBlank - TimeToPixel(SubAreaTimes.Enter + GetRelativeTimeToSegment(timeAfterOffset, segmentId));
        public float OffsetAndGetY(float time, int segmentId)
            => GetY(Offset(time), segmentId);

        public float GetLeftSideXOfSegment(int segmentId)
            => Padding.Left + segmentId * (SegmentImageInfo.Width + GapWidth);

        public void Dispose()
        {
            Assets.Dispose();
        }
    }
}
