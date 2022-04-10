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
        
        public DocumentationGroup(in DocumentationSnippet documentationSnippet) : this(documentationSnippet.GroupInfo.Group)
        {
            documents.Add(documentationSnippet);
        }

        public void TryAddToGroup(in DocumentationSnippet documentationSnippet)
        {
            if (documentationSnippet.GroupInfo.Group == Group)
            {
                documents.Add(documentationSnippet);
            }
        }
        
        public void AddToGroup(in DocumentationSnippet documentationSnippet)
        {
            Assert.IsTrue(documentationSnippet.GroupInfo.Group == Group);
            documents.Add(documentationSnippet);
        }

        public List<DocumentationSnippet> GetSnippets()
        {
            if (Group != Grouping.None)
            {
                documents.Sort(DocumentationSnippet.Comparer);
                CheckForDuplicates(documents);
            }
            return documents;
        }

        public int Compare(DocumentationGroup x, DocumentationGroup y)
        {
            return x.Group.CompareTo(y.Group);
        }

        private static void CheckForDuplicates(List<DocumentationSnippet> sortedDocuments)
        {
            DocumentationSnippet? previousDocument = null;
            foreach (DocumentationSnippet document in sortedDocuments)
            {
                if (!previousDocument.HasValue)
                {
                    previousDocument = document;
                    continue;
                }

                if (previousDocument.Value.GroupInfo.IndexInGroup == document.GroupInfo.IndexInGroup)
                {
                    string subject = document.MemberBeingDocumented.GetReadableName();
                    string index = document.GroupInfo.IndexInGroup.ToString();
                    string memberA = previousDocument.Value.MemberDoingTheDocumenting.GetReadableName();
                    string memberB = document.MemberDoingTheDocumenting.GetReadableName();
                    throw new InvalidOperationException(nameof(CheckForDuplicates).ErrorContext<DocumentationGroup>(true) +
                                                        $"The following demonstrating members of ${subject} " +
                                                        $"both share ${index} in {document.Group}:" +
                                                        $"{Environment.NewLine}- ${memberA}" +
                                                        $"{Environment.NewLine}- ${memberB}");
                }
            }
        }
    }
}