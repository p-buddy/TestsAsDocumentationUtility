using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct Documentation
    {
        /// <summary>
        /// 
        /// </summary>
        public Grouping Group { get; }

        /// <summary>
        /// 
        /// </summary>
        public IndexInGroup IndexInGroup { get; }
        
        /// <summary>
        ///
        /// </summary>
        public MemberInfo MemberBeingDocumnted { get; }

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

        public Documentation(MemberInfo documentationSubject,
                             string title,
                             string description,
                             string containingFile,
                             LineNumberRange attributeLineNumberRange,
                             RelevantArea relevantArea,
                             MemberInfo memberDoingTheDocumenting,
                             Grouping group,
                             IndexInGroup indexInGroup)
        {
            MemberBeingDocumnted = documentationSubject;
            Title = title;
            Description = description;
            ContainingFile = containingFile;
            Group = group;
            IndexInGroup = indexInGroup;
            MemberDoingTheDocumenting = memberDoingTheDocumenting;
            DocumentationLineNumberRange = FileParser.GetLineNumberRangeForMember(memberDoingTheDocumenting,
                containingFile,
                relevantArea,
                attributeLineNumberRange.End);
        }

        public string GetContents()
        {
            string[] lines = File.ReadAllLines(ContainingFile);
            int index = DocumentationLineNumberRange.Start - 1;
            int length = DocumentationLineNumberRange.End - index;
            return String.Join(Environment.NewLine, new ArraySegment<string>(lines, index, length));
        }

        public string GenerateMarkdown()
        {
            static string ToTitle(string text) => $"# {text}{Environment.NewLine}";
            static string ToDescription(string text) => $"{text}{Environment.NewLine}";
            static string ToCode(string text) => $"```csharp{Environment.NewLine}{text}{Environment.NewLine}```{Environment.NewLine}";
            
            static string ToMarkdown(string text, Func<string, string> converter)
            {
                if (String.IsNullOrEmpty(text))
                {
                    return "";
                }

                return converter.Invoke(text);
            }

            var builder = new StringBuilder();
            builder.Append(ToMarkdown(Title, ToTitle));
            builder.Append(ToMarkdown(Description, ToDescription));
            builder.Append(ToMarkdown(GetContents(), ToCode));
            
            return builder.ToString();
        }
        
        
    }
}