namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public interface IHighlighter
    {
        void HighlightNext(uint numberOfLines = 1, string demoMsg = null);
    }
}