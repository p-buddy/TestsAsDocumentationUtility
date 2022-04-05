using pbuddy.TestsAsDocumentationUtility.EditorScripts;

namespace pbuddy.TestsAsDocumentationUtility.Generator
{
    public abstract class TestAsDocumentationBase : IHighlighter
    {
        public void HighlightNext(uint numberOfLines = 1, string demoMsg = null) { }
    }
}