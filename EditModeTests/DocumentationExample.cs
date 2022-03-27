using System.Reflection;
using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts;
using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class DocumentationTestExample 
    {
        [Test]
        [Demonstrates(typeof(DocumentedByTestExample), nameof(DocumentedByTestExample.SomeMethod))]
        public void TestOfSomeMethod()
        {
            
        }
        
        [Test]
        [Demonstrates(typeof(DocumentedByTestExample), nameof(DocumentedByTestExample.SomeProperty), RelevantArea.BodyOnly, Grouping.Group0, IndexInGroup.Index0 )]
        public void TestOfSomeProperty()
        {
            
        }


        [Test]
        public void T()
        {
            var m = typeof(DocumentedByTestExample) as MemberInfo;
        }
    }

    public class DocumentedByTestExample
    {
        /// <summary>
        /// 
        /// </summary>
        [DemonstratedBy]
        public int SomeProperty { get; set; }
        
        /// <summary>
        /// Hello
        /// </summary>
        [DemonstratedBy("File: ../../EditModeTests/DocumentationExample.cs", "Line range: 26-30")]
        [DemonstratedBy("Title: This is how", "File: ../../EditModeTests/DocumentationExample.cs", "Line numbers: 26-30")]
        [DemonstratedBy("Title: This is how", "Description: A way to do it", "File: ../../EditModeTests/DocumentationExample.cs", "Line numbers: 26-30")]
        [DemonstratedBy(new []
        {
            "File: ../../EditModeTests/DocumentationExample.cs", "Line(s): 26-30",
            "File: ../../EditModeTests/DocumentationExample.cs", "Line(s): 26-30",
        })]
        public void SomeMethod()
        {
            
        }
    }
}