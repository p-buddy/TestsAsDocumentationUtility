using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    [IsDemonstratedByTests]
    public class DemonstratesAttribute : Attribute, ILineNumberRangeProvider
    {
        private const string FilledInByCompiler = "[Filled in by Compiler]";
        private const int InvalidLineNumber = -1;
        public MemberInfo MemberBeingDemonstrated => memberBeingDemonstrated;
        public LineNumberRange LineNumberRange => attributeLineNumberRange;
        
        private const string ErrorContext = "[" + nameof(DemonstratesAttribute) + " ERROR]: ";

        private readonly string filePath;
        private readonly string title;
        private readonly string description;
        private readonly string groupTitle;
        private readonly string groupDescription;
        private readonly LineNumberRange attributeLineNumberRange;
        private readonly Grouping grouping;
        private readonly IndexInGroup indexInGroup;
        private readonly RelevantArea relevantArea;
        private readonly MemberInfo memberBeingDemonstrated;

        private DemonstratesAttribute(Grouping grouping,
                                      IndexInGroup indexInGroup,
                                      string filePath,
                                      int lineNumber,
                                      string title, 
                                      string description,
                                      string groupTitle,
                                      string groupDescription,
                                      RelevantArea relevantArea)
        {
            this.filePath = filePath;
            this.grouping = grouping;
            this.indexInGroup = indexInGroup;
            this.title = title;
            this.description = description;
            this.groupTitle = groupTitle;
            this.groupDescription = groupDescription;
            this.relevantArea = relevantArea;
            attributeLineNumberRange = FileParser.GetLineNumberRangeForAttribute(lineNumber, filePath);
        }
        
        #region Public Constructors

        /// <summary>
        /// Constructor for documenting a type.
        /// </summary>
        /// <param name="typeOfThingBeingDemonstrated"></param>
        /// <param name="relevantArea"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="grouping"></param>
        /// <param name="indexInGroup"></param>
        /// <param name="groupTitle"></param>
        /// <param name="groupDescription"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        public DemonstratesAttribute(Type typeOfThingBeingDemonstrated,
                                     RelevantArea relevantArea = RelevantArea.BodyOnly,
                                     string title = null,
                                     string description = null,
                                     Grouping grouping = Grouping.None,
                                     IndexInGroup indexInGroup = IndexInGroup.Default,
                                     string groupTitle = null,
                                     string groupDescription = null,
                                     [CallerFilePath] string file = FilledInByCompiler, 
                                     [CallerLineNumber] int line = InvalidLineNumber) 
            : this(grouping,
                   indexInGroup,
                   file,
                   line,
                   title,
                   description,
                   groupTitle,
                   groupDescription,
                   relevantArea)
        {
            memberBeingDemonstrated = typeOfThingBeingDemonstrated;
        }
        
        /// <summary>
        /// Constructor for documenting a non-overloaded member (and thus a member that can be identified using it's name alone)
        /// </summary>
        /// <param name="typeOfThingBeingDemonstrated"></param>
        /// <param name="memberName"></param>
        /// <param name="relevantArea"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="grouping"></param>
        /// <param name="indexInGroup"></param>
        /// <param name="groupTitle"></param>
        /// <param name="groupDescription"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        public DemonstratesAttribute(Type typeOfThingBeingDemonstrated, 
                                     string memberName, 
                                     RelevantArea relevantArea = RelevantArea.BodyOnly,
                                     string title = null,
                                     string description = null,
                                     Grouping grouping = Grouping.None,
                                     IndexInGroup indexInGroup = IndexInGroup.Default,
                                     string groupTitle = null,
                                     string groupDescription = null,
                                     [CallerFilePath] string file = FilledInByCompiler, 
                                     [CallerLineNumber] int line = InvalidLineNumber) 
            : this(grouping, 
                   indexInGroup, 
                   file, 
                   line, 
                   title, 
                   description,
                   groupTitle,
                   groupDescription,
                   relevantArea)
        {
            bool result = typeOfThingBeingDemonstrated.TryGetNonOverloadedMember(memberName,
                out memberBeingDemonstrated,
                out string errorMsg);
            
            Assert.IsTrue(result, $"{ErrorContext}: {errorMsg}");
        }
        
        /// <summary>
        /// Constructor for documenting an overloaded member (which therefore requires specifying the argument types that
        /// can be used to uniquely identify the member).
        /// </summary>
        /// <param name="typeOfThingBeingDemonstrated"></param>
        /// <param name="memberName"></param>
        /// <param name="argumentTypes"></param>
        /// <param name="relevantArea"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="grouping"></param>
        /// <param name="indexInGroup"></param>
        /// <param name="groupTitle"></param>
        /// <param name="groupDescription"></param>
        /// <param name="file"></param>
        /// <param name="line"></param>
        public DemonstratesAttribute(Type typeOfThingBeingDemonstrated, 
                                     string memberName, 
                                     Type[] argumentTypes,
                                     RelevantArea relevantArea = RelevantArea.BodyOnly,
                                     string title = null,
                                     string description = null,
                                     Grouping grouping = Grouping.None,
                                     IndexInGroup indexInGroup = IndexInGroup.Default,
                                     string groupTitle = null,
                                     string groupDescription = null,
                                     [CallerFilePath] string file = FilledInByCompiler, 
                                     [CallerLineNumber] int line = InvalidLineNumber) 
            : this(grouping, 
                   indexInGroup, 
                   file, 
                   line, 
                   title, 
                   description,
                   groupTitle,
                   groupDescription,
                   relevantArea)
        {
            bool result = typeOfThingBeingDemonstrated.TryGetOverloadedMember(memberName,
                                                                              argumentTypes,
                                                                              out memberBeingDemonstrated,
                                                                              out string errorMsg);
            Assert.IsTrue(result, $"{ErrorContext}: {errorMsg}");
        }
        #endregion Public Constructors

        public DocumentationSnippet GetSnippet(MemberInfo memberDoingTheDocumenting)
        {
            return new DocumentationSnippet(memberBeingDemonstrated,
                                     title,
                                     description,
                                     filePath,
                                     attributeLineNumberRange,
                                     relevantArea,
                                     memberDoingTheDocumenting,
                                     grouping,
                                     indexInGroup,
                                     groupTitle,
                                     groupDescription);
        }

        public bool TryGetDocumentationSubject(out DocumentationSubject thingsBeingDocumented)
        {
            if (DocumentationSubject.TryCreate(memberBeingDemonstrated, out thingsBeingDocumented, out string error))
            {
                return true;
            }
            
            Debug.LogError(error);
            return false;
        }
    }
}