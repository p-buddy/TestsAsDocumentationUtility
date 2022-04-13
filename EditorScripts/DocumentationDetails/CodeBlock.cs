namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct CodeBlock
    {
        public HighlightRegion[] HighlightRegions { get; }
        public string Title { get; }
        public string Description { get; }
        public string Content { get; }
        
        private CodeBlock(string content, HighlightRegion[] highlightRegions, string title = null, string description = null)
        {
            Content = content;
            HighlightRegions = highlightRegions;
            Title = title;
            Description = description;
        }

        public CodeBlock NullifyTitleAndDescription() => new CodeBlock(Content, HighlightRegions);
    }
}