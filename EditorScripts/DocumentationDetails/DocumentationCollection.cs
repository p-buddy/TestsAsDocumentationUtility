using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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

        public string GetXML()
        {
            List<DocumentationGroup> groups = groupingMap.Values.ToList();
            groups.Sort(DocumentationGroup.Comparer);
            string[] xmlPerGroup = new string[groups.Count];
            
            int callCount = 0;
            string ToTitle(string text)
            {
                string title = $"Example {callCount + 1}: {text}";
                callCount++;
                return title;
            }
            
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].Group == Grouping.None)
                {
                    xmlPerGroup[i] = groups[i].GenerateXML(ToTitle);
                }
            }

            return String.Join(Environment.NewLine, xmlPerGroup);
        }
    }
}