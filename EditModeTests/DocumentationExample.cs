using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts;
using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class DocumentationTestExample : TestsAsDocumentationBase
    {
        [Test]
        [Demonstrates(typeof(DocumentedByTestExample), nameof(DocumentedByTestExample.SomeMethod))]
        public void TestOfSomeMethod()
        {
            
        }
        
        [Test]
        [Demonstrates(typeof(DocumentedByTestExample), nameof(DocumentedByTestExample.SomeProperty))]
        public void TestOfSomeProperty()
        {
            
        }
    }

    public class DocumentedByTestExample
    {
        [DemonstratedBy]
        public int SomeProperty { get; set; }
        
        [DemonstratedBy]
        public void SomeMethod()
        {
            
        }
    }
}