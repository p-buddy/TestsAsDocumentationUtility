namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public struct Removal : IMarkup
    {
        public MarkupType Type { get; }
        public LineNumberRange SelfRange { get; }
        public AppliesTo? AppliesTo { get; }
        public string Filepath { get; }
        public uint Amount { get; }

        public Removal(string file, int line, uint amount, AppliesTo? appliesTo)
        {
            Type = MarkupType.Removal;
            AppliesTo = appliesTo;
            Amount = amount;
            Filepath = file;
            SelfRange = FileParser.GetRangeBetweenCharacters(file, line, CharacterPair.Parenthesis, true);
        }
    }
}