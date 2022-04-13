using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct DocumentationSnippet: IComparer<DocumentationSnippet>, IEqualityComparer<DocumentationSnippet>
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
            CheckValidGroup(memberDoingTheDocumenting, group);
            GroupInfo = new GroupInfo(group, indexInGroup, groupTitle, groupDescription, documentationSubject);
            DocumentationLineNumberRange = FileParser.GetLineNumberRangeForMember(memberDoingTheDocumenting,
                containingFile,
                relevantArea,
                attributeLineNumberRange.End);
        }
        
        /// <summary>
        /// Compare two <see cref="DocumentationSnippet"/>s (assumed to be in the same Group)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(DocumentationSnippet x, DocumentationSnippet y)
        {
            Assert.AreEqual(x.GroupInfo.Group, y.GroupInfo.Group);
            return ((int)x.GroupInfo.IndexInGroup).CompareTo((int)y.GroupInfo.IndexInGroup);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetContent()
        {
            IEnumerable<string> trimmed = GetLines();
            return String.Join(Environment.NewLine, trimmed);
        }
        
        public CodeBlock GetCodeBlock(List<IMarkup> markups)
        {
            return default;
        }

        public IEnumerable<string> GetLines()
        {
            string[] lines = File.ReadAllLines(ContainingFile);
            int index = DocumentationLineNumberRange.Start - 1;
            int length = DocumentationLineNumberRange.End - index;
            var section = new ArraySegment<string>(lines, index, length);
            string whitespace = GetLeadingWhiteSpace(section);
            return section.Select(line => line.RemoveSubString(whitespace));
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
    
        
        private static void CheckValidGroup(MemberInfo member, Grouping group)
        {
            if (group == Grouping.None)
            {
                if (member.GetCustomAttribute<TestAttribute>() == null ||
                    member.GetCustomAttribute<UnityTestAttribute>() == null)
                {
                    string context = nameof(CheckValidGroup).ErrorContext<DocumentationSnippet>(true);
                    string msg = $"{member.GetTypeRecoverableName()} is an auxiliary demonstrating member " +
                                 $"(meaning it is marked with a {nameof(DemonstratesAttribute)}, " +
                                 $"but neither a {nameof(TestAttribute)} nor {nameof(UnityTestAttribute)}) " +
                                 $"and thus should not be in the ${Grouping.None} group. " +
                                 $"Using the appropriate {nameof(DemonstratesAttribute)} constructor, " +
                                 "designate it (and the demonstrating test that uses it) to be in an appropriate group.";
                    throw new InvalidOperationException(context + msg);
                }
            }
        }

        public void ApplyMarkup(List<IMarkup> markups)
        {
            
        }

        public bool Equals(DocumentationSnippet x, DocumentationSnippet y)
        {
            return x.GroupInfo.Equals(y.GroupInfo) &&
                   Equals(x.MemberDoingTheDocumenting, y.MemberDoingTheDocumenting) &&
                   x.Title == y.Title &&
                   x.Description == y.Description &&
                   x.ContainingFile == y.ContainingFile &&
                   x.DocumentationLineNumberRange.Equals(y.DocumentationLineNumberRange);
        }

        public int GetHashCode(DocumentationSnippet obj)
        {
            unchecked
            {
                var hashCode = obj.GroupInfo.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.MemberDoingTheDocumenting != null ? obj.MemberDoingTheDocumenting.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Title != null ? obj.Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.Description != null ? obj.Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.ContainingFile != null ? obj.ContainingFile.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.DocumentationLineNumberRange.GetHashCode();
                return hashCode;
            }
        }
    }
}