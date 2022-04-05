using System.Reflection;
using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts;
using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;
using UnityEngine.UI;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class DocumentationTestExample
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
        [Demonstrates(typeof(DocumentedByTestExample),
                      nameof(DocumentedByTestExample.SomeProperty),
                      RelevantArea.DeclarationAndBodyAndBelowAttributes,
                      null, 
                      null,
                      Grouping.Group0,
                      IndexInGroup.Index0)]
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
        /// Hello  <a href="file:///Docs.xml">Demonstrations</a>

        /// </summary>
        /// <include file='./Docs.xml' path='MyDocs/MyMembers[@name="test2"]/*' label="" />
        [IsDemonstratedByTests()]
        public void SomeMethod()
        {
            
        }
    }
}