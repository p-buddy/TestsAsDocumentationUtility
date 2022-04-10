using System;
using System.Collections.Generic;
using System.Linq;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public class DocumentationCollection
    {
        private readonly Dictionary<Grouping, DocumentationGroup> groupingMap = new Dictionary<Grouping, DocumentationGroup>();

        public DocumentationCollection(in DocumentationSnippet doc)
        {
            Add(in doc);
        }
        
        public DocumentationCollection(params DocumentationSnippet[] docs)
        {
            foreach (DocumentationSnippet doc in docs)
            {
                Add(in doc);
            }
        }
        
        public DocumentationCollection(IEnumerable<DocumentationSnippet> docs)
        {
            foreach (DocumentationSnippet doc in docs)
            {
                Add(in doc);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        public void Add(in DocumentationSnippet doc)
        {
            if (groupingMap.TryGetValue(doc.Group, out DocumentationGroup group))
            {
                group.AddToGroup(doc);
                return;
            }

            groupingMap[doc.Group] = new DocumentationGroup(doc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DocumentationGroup[] GetGroups()
        {
            List<DocumentationGroup> groups = groupingMap.Values.ToList();
            groups.Sort(DocumentationGroup.Comparer);
            return groups.ToArray();
        }
    }
}