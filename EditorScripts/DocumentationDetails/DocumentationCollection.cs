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
        private readonly Dictionary<Grouping, DocumentationGroup> groupingMap = new Dictionary<Grouping, DocumentationGroup>();
        private static readonly Grouping[] GroupNames = Enum.GetValues(typeof(Grouping)) as Grouping[];

        public DocumentationCollection(in Documentation doc)
        {
            Add(doc);
        }

        public void Add(in Documentation doc)
        {
            if (groupingMap.TryGetValue(doc.Group, out DocumentationGroup group))
            {
                group.AddToGroup(doc);
                return;
            }

            groupingMap[doc.Group] = new DocumentationGroup(doc);
        }

        public DocumentationGroup[] GetGroups()
        {
            List<DocumentationGroup> documentation = new List<DocumentationGroup>();
            foreach (Grouping groupName in GroupNames)
            {
                if (groupingMap.TryGetValue(groupName, out DocumentationGroup group))
                {
                    if (groupName == Grouping.Default)
                    {
                        // Split out default documents into their own separate groups
                        group.GetDocuments().ForEach(doc => documentation.Add(new DocumentationGroup(doc)));
                    }
                    else
                    {
                        documentation.Add(group);
                    }
                }
            }

            return documentation.ToArray();
        }
    }
}