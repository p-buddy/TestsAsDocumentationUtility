using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts;

namespace pbuddy.TestsAsDocumentationUtility.Generator
{
    public class Generator
    {
        [Test]
        public void GenerateDocumentation()
        {
            TestsAsDocumentationFactory.CreateDocumentation();
        }
    }
}