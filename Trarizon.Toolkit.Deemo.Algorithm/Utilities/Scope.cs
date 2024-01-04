using System.Diagnostics;

namespace Trarizon.Toolkit.Deemo.Algorithm.Utilities;
/// <summary>
/// 表示一个可填充单色的区间，初始状态为填充
/// </summary>
internal struct Scope(float start, float end)
{
    private bool _beginWithBlank = false;
    private readonly List<float> _seperators = [start, end];

    public readonly bool IsBlank => _seperators.Count == 2 && _beginWithBlank;

    /// <summary>
    /// 将区间中的指定子区间进行擦除
    /// </summary>
    public void Exclude(float start, float end)
    {
        if (start >= end) return;

        if (start > _seperators[^1] ||
            end < _seperators[0])
            return;


        bool isCurrentBlankBegin;
        int startIndex, endIndex;

        // Find start
        isCurrentBlankBegin = _beginWithBlank;
        // If start < first, 指定区间开始于总区间开头，startIndex = 0
        startIndex = 0;
        if (start > _seperators[0]) {
            // 找到>=指定开始位置的第一个index
            while (_seperators[startIndex] < start) {
                startIndex++;
                isCurrentBlankBegin = !isCurrentBlankBegin;
            }

            // ----|
            //  ^s ^i
            // 此时直接插入start，index前移
            // -|  |
            //  ^is
            if (isCurrentBlankBegin) {
                _seperators[startIndex] = start;
            }
            // -|     |---
            //     ^s ^i
            // 此时startIndex移至上一个，翻转isBlank
            // -|     |---
            //  ^i ^s
            else {
                startIndex--;
                isCurrentBlankBegin = !isCurrentBlankBegin;
            }
        }

        // Find end
        if (end < _seperators[^1]) {
            endIndex = startIndex; // 从 startIndex开始查找
            while (_seperators[endIndex] < end) {
                endIndex++;
                isCurrentBlankBegin = !isCurrentBlankBegin;
            }

            // ----|
            // ^e ^i
            // 此时在e处插入新标记，记为index
            // |---|
            // ^ie ^next
            if (isCurrentBlankBegin) {
                _seperators.Insert(endIndex, end);
            }
            //    |---
            // ^e ^i
            // 此时无需变动
            else { }
        }
        else { // end at end
            endIndex = _seperators.Count - 1;
        }

        // 如果从头开始，翻转总标记
        if (startIndex == 0)
            _beginWithBlank = true;

        // 清空startIndex到endIndex之间的所有标记
        _seperators.RemoveRange(startIndex + 1, endIndex - startIndex - 1);
    }

    /// <summary>
    /// 获得所有已填色的区间
    /// </summary>
    public readonly Range[] GetFilledRanges()
    {
        // LinkedListNode<float>? start = _seperators.First!;
        int startIndex = 0;
        int count = _seperators.Count;

        if (_beginWithBlank) {
            startIndex++;
            count--;
        }
        count /= 2;

        if (count == 0)
            return [];


        Range[] ranges = new Range[count];
        for (int i = 0; i < count; i++) {
            ranges[i] = new Range(_seperators[startIndex], _seperators[startIndex + 1]);
            startIndex += 2;
        }
        Debug.Assert(startIndex >= _seperators.Count);

        return ranges;
    }

    public readonly struct Range(float start, float end)
    {
        public readonly float Start = start;
        public readonly float Length = end - start;
    }
}
