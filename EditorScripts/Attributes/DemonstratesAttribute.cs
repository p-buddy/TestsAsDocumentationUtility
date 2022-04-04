using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Assertions;

using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DemonstratesAttribute : Attribute, ILineNumberRangeProvider
    {
        private const string FilledInByCompiler = "[Filled in by Compiler]";
        private const int InvalidLineNumber = -1;
        public LineNumberRange LineNumberRange => attributeLineNumberRange;
        
        private const string ErrorContext = "[" + nameof(DemonstratesAttribute) + " ERROR]: ";
        private const BindingFlags ComprehensiveFlags = BindingFlags.Public | 
                                                        BindingFlags.NonPublic | 
                                                        BindingFlags.Static | 
                                                        BindingFlags.Instance | 
                                                        BindingFlags.DeclaredOnly;
        private readonly string filePath;
        private readonly string title;
        private readonly string description;
        private readonly string groupTitle;
        private readonly string groupDescription;
        private readonly LineNumberRange attributeLineNumberRange; 
        private readonly Grouping grouping;
        private readonly IndexInGroup indexInGroup;
        private readonly RelevantArea relevantArea;
        private readonly Type typeOfThingBeingDemonstrated;
        private readonly MemberInfo memberInfoOfThingBeingDemonstrated;

        private DemonstratesAttribute(Type typeOfThingBeingDemonstrated,
                                      Grouping grouping,
                                      IndexInGroup indexInGroup,
                                      string filePath,
                                      int lineNumber,
                                      string title, 
                                      string description,
                                      string groupTitle,
                                      string groupDescription,
                                      RelevantArea relevantArea)
        {
            this.typeOfThingBeingDemonstrated = typeOfThingBeingDemonstrated;
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
            : this(typeOfThingBeingDemonstrated,
                   grouping,
                   indexInGroup,
                   file,
                   line,
                   title,
                   description,
                   groupTitle,
                   groupDescription,
                   relevantArea)
        {
            memberInfoOfThingBeingDemonstrated = typeOfThingBeingDemonstrated;
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
            : this(typeOfThingBeingDemonstrated, 
                   grouping, 
                   indexInGroup, 
                   file, 
                   line, 
                   title, 
                   description,
                   groupTitle,
                   groupDescription,
                   relevantArea)
        {
            Assert.IsTrue(TryGetNonOverloadedMember(typeOfThingBeingDemonstrated,
                                                    memberName,
                                                    out memberInfoOfThingBeingDemonstrated,
                                                    out string errorMsg),
                          $"{ErrorContext}: {errorMsg}");
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
            : this(typeOfThingBeingDemonstrated, 
                   grouping, 
                   indexInGroup, 
                   file, 
                   line, 
                   title, 
                   description,
                   groupTitle,
                   groupDescription,
                   relevantArea)
        {
            Assert.IsTrue(TryGetOverloadedMember(typeOfThingBeingDemonstrated,
                                                 memberName,
                                                 argumentTypes,
                                                 out memberInfoOfThingBeingDemonstrated,
                                                 out string errorMsg),
                          $"{ErrorContext}: {errorMsg}");
        }
        #endregion Public Constructors

        public Documentation GetDocument(MemberInfo memberDoingTheDocumenting)
        {
            return new Documentation(memberInfoOfThingBeingDemonstrated,
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
            if (DocumentationSubject.TryCreate(memberInfoOfThingBeingDemonstrated, out thingsBeingDocumented, out string error))
            {
                return true;
            }
            
            Debug.LogError(error);
            return false;
        }
        
        private static bool TryGetNonOverloadedMember(Type type, string memberName, out MemberInfo memberInfo, out string errorMsg)
        {
            MemberInfo[] members = type.GetMember(memberName, ComprehensiveFlags);
            if (members.Length == 0)
            {
                memberInfo = default;
                errorMsg = $"Unable to locate member '{memberName}' on {type.Name} type";
                return false;
            }
            
            if (members.Length == 1)
            {
                memberInfo = members[0];
                errorMsg = default;
                return true;
            }

            foreach (MemberInfo member in members)
            {
                if (member.MemberType != MemberTypes.Method || new MemberMethodProbe(member).ArgumentCount != 0)
                {
                    continue;
                }

                memberInfo = member;
                errorMsg = default;
                return true;
            }
            
            memberInfo = default;
            errorMsg = $"There were multiple member methods named '{memberName}' on {type.Name} type. " +
                       $"Use an alternative {nameof(DemonstratesAttribute)} constructor to better specify which member you are targeting.";
            return false;
        }


        private static bool TryGetOverloadedMember(Type type,
                                                   string memberName,
                                                   Type[] argumentTypes,
                                                   out MemberInfo memberInfo,
                                                   out string errorMsg)
        {
            MemberInfo[] members = type.GetMember(memberName, ComprehensiveFlags);
            if (members.Length == 0)
            {
                memberInfo = default;
                errorMsg = $"Unable to locate member '{memberName}' on {type.Name} type";
                return false;
            }

            foreach (MemberInfo member in members)
            {
                if (member.MemberType != MemberTypes.Method || !new MemberMethodProbe(member).TypesMatch(argumentTypes))
                {
                    continue;
                }

                memberInfo = member;
                errorMsg = default;
                return true;
            }

            IEnumerable<string> argumentsOfOverloads = members.Select(member => new MemberMethodProbe(member))
                                                              .Select(probe => String.Join(", ",
                                                                          probe.DescriptiveArgumentTypeNames))
                                                              .Select(args => $"({args})");

            memberInfo = default;
            errorMsg = $"No method named '{memberName}' with arguments of " +
                       String.Join(", ", argumentTypes.Select(t => t.Name)) + " " +
                       $"could be found on {type.Name} type. " +
                       $"Below are the argument types for all of the overloads of '{memberName}' that do exist:" +
                       String.Join($"\n\t-{memberName}", argumentsOfOverloads);
            return false;
        }
    }
}