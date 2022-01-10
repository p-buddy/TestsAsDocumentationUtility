using System.Reflection;
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

        [Test]
        public override void CreateDocumentation() => InternalCreateDocumentation();

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
        /// <example>
        /// # Howdy
        /// ### Hi hi
        /// [!code-csharp[Main](../../EditModeTests/DocumentationExample.cs?range=34,26-30)]
        /// [!code-csharp[Main](../../EditModeTests/DocumentationExample.cs?range=40-42)]
        /// </example>
        /// /// <example>
        /// [!code-csharp[Main](../../EditModeTests/DocumentationExample.cs?range=34,26-30)]
        /// </example>
        [DemonstratedBy]
        public void SomeMethod()
        {
            
        }
    }
}