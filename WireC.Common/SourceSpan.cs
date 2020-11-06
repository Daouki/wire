namespace WireC.Common
{
    public class SourceSpan
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Length => End - Start;
        public int Line { get; set; }
        public int Column { get; set; }

        public static SourceSpan Merge(SourceSpan start, SourceSpan end) => new SourceSpan
        {
            Start = start.Start,
            End = end.End,
            Line = start.Line,
            Column = start.Column,
        };
    }
}