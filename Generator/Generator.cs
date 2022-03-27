using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts;

namespace pbuddy.TestsAsDocumentationUtility.Genertator
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