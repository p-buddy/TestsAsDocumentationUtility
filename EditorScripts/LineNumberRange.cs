namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct LineNumberRange
    {
        public int Start { get; }
        public int End { get; }

        public LineNumberRange(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}