using System.Collections.Generic;
using NUnit.Framework;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct DocumentationGroup
    {
        private struct DocumentComparer : IComparer<Documentation>
        {
            public int Compare(Documentation x, Documentation y)
            {
                return ((int)x.IndexInGroup).CompareTo((int)y.IndexInGroup);
            }
        }
        
        public Grouping Group { get; }
        private readonly List<Documentation> documents;

        private DocumentationGroup(Grouping group)
        {
            Group = group;
            documents = new List<Documentation>();
        }
        
        public DocumentationGroup(Documentation documentation) : this(documentation.Group)
        {
            documents.Add(documentation);
        }

        public void AddToGroup(Documentation documentation)
        {
            Assert.IsTrue(documentation.Group == Group);
            documents.Add(documentation);
        }

        public List<Documentation> GetDocuments()
        {
            var comparer = new DocumentComparer();
            documents.Sort(comparer);
            return documents;
        }
    }
}