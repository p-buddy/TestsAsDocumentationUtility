namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public struct Addition : IMarkup
    {
        public MarkupType Type { get; }
        public LineNumberRange Range { get; }
        public AppliesTo AppliesTo { get; }
        public string Contents { get; }
        public string Filepath { get; }

        public Addition(string file, int line, string contents, AppliesTo appliesTo)
        {
            Type = MarkupType.Addition;
            AppliesTo = appliesTo;
            Contents = contents;
            Filepath = file;
            Range = FileParser.GetRangeBetweenCharacters(file, line, CharacterPair.Parenthesis, true);
        }
    }
}