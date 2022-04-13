namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct CodeBlock
    {
        public HighlightRegion[] HighlightRegions { get; }
        public string Title { get; }
        public string Description { get; }
        public string Content { get; }
    }
}