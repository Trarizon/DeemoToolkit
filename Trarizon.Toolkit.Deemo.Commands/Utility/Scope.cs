using System.Diagnostics;

namespace Trarizon.Toolkit.Deemo.Commands.Utility;
internal struct Scope
{
    private bool _beginWithBlank;
    private readonly LinkedList<float> _seperators;

    public readonly bool IsBlank => _seperators.Count == 2 && _beginWithBlank;

    public Scope(float start, float end)
    {
        _seperators = new LinkedList<float>();
        _seperators.AddFirst(start);
        _seperators.AddLast(end);
        _beginWithBlank = false;
    }

    public void Exclude(float start, float end)
    {
        if (start >= end) return;

        if (start > _seperators.Last!.Value ||
            end < _seperators.First!.Value)
            return;


        bool isCurrentBlankBegin;
        LinkedListNode<float> startNode, endNode;

        // Find start
        isCurrentBlankBegin = _beginWithBlank;
        if (start <= _seperators.First!.Value) { // start at begin 
            startNode = _seperators.First;
        }
        else {
            startNode = _seperators.First;
            while (startNode.Value < start) {
                startNode = startNode.Next!;
                isCurrentBlankBegin = !isCurrentBlankBegin;
            }

            if (isCurrentBlankBegin) { startNode = _seperators.AddBefore(startNode, start); }
            else { startNode = startNode.Previous!; }
            isCurrentBlankBegin = !isCurrentBlankBegin; // This is the value of startNote.Next, so reverse required
        }

        // Find end
        if (end >= _seperators.Last!.Value) { // end at end
            endNode = _seperators.Last;
        }
        else {
            endNode = startNode;
            while (endNode.Value < end) {
                endNode = endNode.Next!;
                isCurrentBlankBegin = !isCurrentBlankBegin;
            }

            if (isCurrentBlankBegin) { endNode = _seperators.AddBefore(endNode, end); }
            else { /* endNode = endNode; */ }
        }

        // Begin from left boundary
        if (startNode == _seperators.First)
            _beginWithBlank = true;

        // Clear redundant seperators
        while (startNode.Next != endNode) {
            _seperators.Remove(startNode.Next!);
        }
    }

    public readonly Range[] GetFilledRanges()
    {
        LinkedListNode<float>? start = _seperators.First!;
        int count = _seperators.Count;

        if (_beginWithBlank) {
            start = start.Next!;
            count--;
        }
        count /= 2;

        if (count == 0)
            return Array.Empty<Range>();

        Range[] ranges = new Range[count];
        for (int i = 0; i < count; i++) {
            ranges[i] = new Range(start.Value, start.Next!.Value);
            start = start.Next!.Next!;
        }
        Debug.Assert(start == null || start == _seperators.Last);

        return ranges;
    }

    public readonly struct Range
    {
        public readonly float Start;
        public readonly float Length;

        public Range(float start, float end)
        {
            Start = start;
            Length = end - start;
        }
    }
}
