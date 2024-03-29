using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts;
using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;
using pbuddy.TypeUtility.RuntimeScripts;
using UnityEditor;
using UnityEngine.UI;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class DocumentationTestsExample : TestsAsDocumentationBase
    {
        [Test]
        [Demonstrates(typeof(DocumentedByTestExample), RelevantArea.DeclarationAndBody, "How to: " + nameof(DocumentedByTestExample.SomeMethod))]
        [Demonstrates(typeof(DocumentedByTestExample),
                      nameof(DocumentedByTestExample.SomeMethod),
                      RelevantArea.DeclarationAndBody,
                      "Simple " + nameof(DocumentedByTestExample.SomeMethod) + " Example")]
        public void TestOfSomeMethod()
        {
            var x = new DocumentedByTestExample();
            var y = x.SomeProperty;
        }
        
        [Test]
        [Demonstrates(typeOfThingBeingDemonstrated: typeof(DocumentedByTestExample),
                      memberName: nameof(DocumentedByTestExample.SomeProperty),
                      relevantArea: RelevantArea.DeclarationAndBodyAndBelowAttributes,
                      title: null, 
                      description: null,
                      grouping: Grouping.Group0,
                      indexInGroup: IndexInGroup.Index0)]
        //Howdy
        public void TestOfSomeProperty()
        {
            var x = new DocumentedByTestExample();
            x.SomeMethod();
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
        public int SomeProperty { get; set; }
        
        /// <summary>
        /// Hello
        /// </summary>
        /// 
        /// <include file='./Docs.xml' path='MyDocs/MyMembers[@name="test2"]/*' />
        /// 
        [IsDemonstratedByTests()]
        public void SomeMethod()
        {
        }
    }
}