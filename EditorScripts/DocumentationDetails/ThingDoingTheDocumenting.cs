using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct ThingDoingTheDocumenting
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

        public ThingDoingTheDocumenting(string title,
                                        string description,
                                        string containingFile,
                                        LineNumberRange lineNumberRange,
                                        MemberInfo memberDoingTheDocumenting,
                                        Grouping group,
                                        IndexInGroup indexInGroup)
        {
            Title = title;
            Description = description;
            ContainingFile = containingFile;
            Group = group;
            IndexInGroup = indexInGroup;
            MemberDoingTheDocumenting = memberDoingTheDocumenting;
            DocumentationLineNumberRange = lineNumberRange;
        }
    }
}