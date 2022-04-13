using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using NUnit.Framework;
using UnityEngine;

using pbuddy.TestsAsDocumentationUtility.EditorScripts;
using pbuddy.TestsAsDocumentationUtility.Generator;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class DocumentationSnippetTestses : TestsAsDocumentationBase
    {
        [Demonstrates(typeof(DemonstratesAttribute), RelevantArea.DeclarationAndBodyAndBelowAttributes, "The method under test", "", Grouping.Group0, IndexInGroup.Index1)]
        [HighlightNext]
        [Demonstrates(typeof(DocumentationSnippetTestses),
                      nameof(TestFunction),
                      RelevantArea.DeclarationAndBody,
                      "This is where the title goes",
                      "This is where the description goes")]
        public (int, string) TestFunction()
        {
            return GetLineNumberAndFile();
        }
        
        [Test]
        [Demonstrates(typeof(DemonstratesAttribute),
                      RelevantArea.BodyOnly,
                      "The test",
                      "",
                      Grouping.Group0,
                      IndexInGroup.Index2,
                      "Using the " + nameof(DemonstratesAttribute) + " to construct a " + nameof(DocumentationSnippet),
                      "An test ...")]
        public void ConstructFromAttributeTest()
        {
            (int line, string file) returnValue = TestFunction();
            LineNumberRange range = new LineNumberRange(returnValue.line - 2, returnValue.line + 1);
            
            MethodInfo testMethod = GetType().GetMethod(nameof(TestFunction));
            Assert.IsNotNull(testMethod);
            DemonstratesAttribute demonstrates = testMethod.GetCustomAttributes<DemonstratesAttribute>()
                                                           .FirstOrDefault(demo => demo.MemberBeingDemonstrated == testMethod);
            Assert.IsNotNull(demonstrates);

            DocumentationSnippet snippet = demonstrates.GetSnippet(testMethod);
            
            Assert.AreEqual("This is where the title goes", snippet.Title);
            Assert.AreEqual("This is where the description goes", snippet.Description);
            Assert.AreEqual(Grouping.None, snippet.Group);
            Assert.AreEqual(returnValue.file, snippet.ContainingFile);
            Assert.AreEqual(testMethod, snippet.MemberBeingDocumented);
            Assert.AreEqual(range, snippet.DocumentationLineNumberRange);
            
            const string tab = "    ";
            string expectContents = $"public (int, string) {nameof(TestFunction)}()" +
                                    $"{Environment.NewLine}{{{Environment.NewLine}" +
                                    $"{tab}return {nameof(GetLineNumberAndFile)}();" +
                                    $"{Environment.NewLine}}}";

            string contents = snippet.GetContent();
            Assert.AreEqual(expectContents, contents);
        }
        
        [Demonstrates(typeof(DemonstratesAttribute),
                      RelevantArea.DeclarationAndBody,
                      nameof(GetLineNumberAndFile) + " Helper Function",
                      "Function that is called to retrieve the line number of the call site and the name of the enclosing file",
                      Grouping.Group0,
                      IndexInGroup.Index0)]
        public (int line, string file) GetLineNumberAndFile([CallerFilePath] string file = null, [CallerLineNumber] int line = -1)
        {
            return (line, file);
        }

        [Test]
        public void GenerateTheXML()
        {
            MemberInfo GetMember(string methodName) => GetType().GetMethod(methodName);
            bool Match(DemonstratesAttribute demo) => demo.MemberBeingDemonstrated == typeof(DemonstratesAttribute);
            DemonstratesAttribute GetAttribute(MemberInfo member) => member.GetCustomAttributes<DemonstratesAttribute>().FirstOrDefault(Match);
            
            string[] methods = { nameof(TestFunction), nameof(ConstructFromAttributeTest), nameof(GetLineNumberAndFile) };
            List<MemberInfo> members = methods.ToList().Select(GetMember).ToList();
            IEnumerable<DemonstratesAttribute> attributes = members.Select(GetAttribute);

            DocumentationSnippet GetSnippet(DemonstratesAttribute attr, int index) => attr.GetSnippet(members[index]);
            IEnumerable<DocumentationSnippet> snippets = attributes.Select(GetSnippet);
            DocumentationCollection collection = new DocumentationCollection(snippets.ToArray());
            
            Debug.Log(collection.GenerateXML());
        }
    }
}