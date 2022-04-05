using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct DocumentationSnippet: IComparer<DocumentationSnippet>
    {
        /// <summary>
        /// 
        /// </summary>
        public static IComparer<DocumentationSnippet> Comparer => new DocumentationSnippet();

        /// <summary>
        /// 
        /// </summary>
        public GroupInfo GroupInfo { get; }

        /// <summary>
        /// 
        /// </summary>
        public Grouping Group => GroupInfo.Group;
        
        /// <summary>
        /// 
        /// </summary>
        public MemberInfo MemberBeingDocumented => GroupInfo.MemberBeingDocumented;


        /// <summary>
        /// 
        /// </summary>
        public MemberInfo MemberDoingTheDocumenting { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// 
        /// </summary>
        public string ContainingFile { get; }

        /// <summary>
        /// 
        /// </summary>
        public LineNumberRange DocumentationLineNumberRange { get; }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentationSubject"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="containingFile"></param>
        /// <param name="attributeLineNumberRange"></param>
        /// <param name="relevantArea"></param>
        /// <param name="memberDoingTheDocumenting"></param>
        /// <param name="group"></param>
        /// <param name="indexInGroup"></param>
        /// <param name="groupTitle"></param>
        /// <param name="groupDescription"></param>
        public DocumentationSnippet(MemberInfo documentationSubject, 
                                    string title,
                                    string description, 
                                    string containingFile,
                                    LineNumberRange attributeLineNumberRange,
                                    RelevantArea relevantArea,
                                    MemberInfo memberDoingTheDocumenting,
                                    Grouping group,
                                    IndexInGroup indexInGroup,
                                    string groupTitle,
                                    string groupDescription)
        {
            Title = title;
            Description = description;
            ContainingFile = containingFile;
            MemberDoingTheDocumenting = memberDoingTheDocumenting;
            GroupInfo = new GroupInfo(group, indexInGroup, groupTitle, groupDescription, documentationSubject);
            DocumentationLineNumberRange = FileParser.GetLineNumberRangeForMember(memberDoingTheDocumenting,
                containingFile,
                relevantArea,
                attributeLineNumberRange.End);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(DocumentationSnippet x, DocumentationSnippet y)
        {
            return ((int)x.GroupInfo.IndexInGroup).CompareTo((int)y.GroupInfo.IndexInGroup);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetContents()
        {
            string[] lines = File.ReadAllLines(ContainingFile);
            int index = DocumentationLineNumberRange.Start - 1;
            int length = DocumentationLineNumberRange.End - index;
            var section = new ArraySegment<string>(lines, index, length);
            string whitespace = GetLeadingWhiteSpace(section);
            IEnumerable<string> trimmed = section.Select(line => line.RemoveSubString(whitespace));
            return String.Join(Environment.NewLine, trimmed);
        }

        private string GetLeadingWhiteSpace(IEnumerable<string> lines)
        {
            foreach (string line in lines)
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                
                return line.GetLeadingWhitespace();
            }

            throw new Exception("Could not retrieve leading whitespace!");
        }
    }
}