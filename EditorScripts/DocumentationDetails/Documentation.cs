using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

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
        
        
    }
}