using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct Documentation: IComparer<Documentation>
    {
        public static IComparer<Documentation> Comparer => new Documentation();

        public GroupInfo GroupInfo { get; }

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
        public Documentation(MemberInfo documentationSubject,
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
        public int Compare(Documentation x, Documentation y)
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
            string whitespace = default;
            int index = DocumentationLineNumberRange.Start - 1;
            int length = DocumentationLineNumberRange.End - index;
            
            foreach (string line in new ArraySegment<string>(lines, index, length))
            {
                if (String.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                
                whitespace = line.GetLeadingWhitespace();
                break;
            }

            IEnumerable<string> trimmed = lines.ToList().Select(line => line.RemoveSubString(whitespace));
            return String.Join(Environment.NewLine, trimmed);
        }
    }
}