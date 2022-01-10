using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;
using UnityEditor.SceneManagement;
using UnityEngine.Assertions;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public class DocumentationCollection
    {
        private struct DocumentComparer : IComparer<ThingDoingTheDocumenting>
        {
            public int Compare(ThingDoingTheDocumenting x, ThingDoingTheDocumenting y)
            {
                return ((int)x.IndexInGroup).CompareTo((int)y.IndexInGroup);
            }
        }
        
        private readonly Dictionary<Grouping, List<ThingDoingTheDocumenting>> groupingMap = new Dictionary<Grouping, List<ThingDoingTheDocumenting>>();
        private static readonly Grouping[] GroupNames = Enum.GetValues(typeof(Grouping)) as Grouping[];

        public void Add(in ThingDoingTheDocumenting doer)
        {
            if (groupingMap.TryGetValue(doer.Group, out List<ThingDoingTheDocumenting> documents))
            {
                documents.Add(doer);
            }
        }

        public ThingDoingTheDocumenting[][] Get()
        {
            DocumentComparer comparer = new DocumentComparer();
            List<ThingDoingTheDocumenting[]> documentation = new List<ThingDoingTheDocumenting[]>();
            foreach (Grouping groupName in GroupNames)
            {
                if (groupingMap.TryGetValue(groupName, out List<ThingDoingTheDocumenting> documents))
                {
                    documents.Sort(comparer);
                    if (groupName == Grouping.Default)
                    {
                        documents.ForEach(doc => documentation.Add(new []{doc}));
                    }
                    else
                    {
                        documentation.Add(documents.ToArray());
                    }
                }
            }

            return documentation.ToArray();
        }
    }
}