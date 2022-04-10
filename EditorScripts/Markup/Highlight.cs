namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct Highlight : IMarkup
    {
        public MarkupType Type { get; }
        public LineNumberRange Range { get; }
        public AppliesTo? AppliesTo { get; }
        public string Filepath { get; }
        public string Comment { get; }
        public uint Amount { get; }


        public Highlight(string file, int line, string comment, uint amount, AppliesTo? appliesTo)
        {
            Type = MarkupType.Highlight;
            AppliesTo = appliesTo;
            Comment = comment;
            Amount = amount;
            Filepath = file;
            Range = FileParser.GetRangeBetweenCharacters(file, line, CharacterPair.Parenthesis, true);
        }
    }
}