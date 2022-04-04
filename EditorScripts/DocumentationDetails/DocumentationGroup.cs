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
        public int Count => documents.Count;
        private readonly List<Documentation> documents;

        private DocumentationGroup(Grouping group)
        {
            Group = group;
            documents = new List<Documentation>();
        }
        
        public DocumentationGroup(Documentation documentation) : this(documentation.GroupInfo.Group)
        {
            documents.Add(documentation);
        }

        public void AddToGroup(Documentation documentation)
        {
            Assert.IsTrue(documentation.GroupInfo.Group == Group);
            documents.Add(documentation);
        }

        public List<Documentation> GetDocuments()
        {
            if (Group != Grouping.None)
            {
                documents.Sort(Documentation.Comparer);
            }
            return documents;
        }

        public int Compare(DocumentationGroup x, DocumentationGroup y)
        {
            return x.Group.CompareTo(y.Group);
        }
    }
}