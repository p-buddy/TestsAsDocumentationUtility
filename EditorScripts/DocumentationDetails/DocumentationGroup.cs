using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct DocumentationGroup : IComparer<DocumentationGroup>
    {
        public static IComparer<DocumentationGroup> Comparer => new DocumentationGroup();
        public Grouping Group { get; }
        private readonly List<DocumentationSnippet> documents;

        private DocumentationGroup(Grouping group)
        {
            Group = group;
            documents = new List<DocumentationSnippet>();
        }
        
        public DocumentationGroup(DocumentationSnippet documentationSnippet) : this(documentationSnippet.GroupInfo.Group)
        {
            documents.Add(documentationSnippet);
        }

        public void AddToGroup(DocumentationSnippet documentationSnippet)
        {
            Assert.IsTrue(documentationSnippet.GroupInfo.Group == Group);
            documents.Add(documentationSnippet);
        }

        public List<DocumentationSnippet> GetSnippets()
        {
            if (Group != Grouping.None)
            {
                documents.Sort(DocumentationSnippet.Comparer);
            }
            return documents;
        }

        public int Compare(DocumentationGroup x, DocumentationGroup y)
        {
            return x.Group.CompareTo(y.Group);
        }
    }
}