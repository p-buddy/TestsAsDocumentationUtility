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
    public class DemonstratesAttribute : Attribute
    { 
        private const string ErrorContext = "[" + nameof(DemonstratesAttribute) + " ERROR]: ";
        private const BindingFlags ComprehensiveFlags = BindingFlags.Public | 
                                                        BindingFlags.NonPublic | 
                                                        BindingFlags.Static | 
                                                        BindingFlags.Instance | 
                                                        BindingFlags.DeclaredOnly;
        private readonly string filePath;
        private readonly string title;
        private readonly string description;
        private readonly Grouping grouping;
        private readonly IndexInGroup indexInGroup;
        private readonly LineNumberRange lineNumberRange;
        private readonly Type typeOfThingBeingDemonstrated;
        private readonly MemberInfo memberInfoOfThingBeingDemonstrated;

        private DemonstratesAttribute(Type typeOfThingBeingDemonstrated,
                                      Grouping grouping,
                                      IndexInGroup indexInGroup,
                                      string filePath,
                                      int lineNumber,
                                      string title, 
                                      string description,
                                      RelevantArea relevantArea)
        {
            this.typeOfThingBeingDemonstrated = typeOfThingBeingDemonstrated;
            this.filePath = filePath;
            this.grouping = grouping;
            this.indexInGroup = indexInGroup;
            this.title = title;
            this.description = description;
            lineNumberRange =  FileReaderHelper.GetLineNumberRange(lineNumber, filePath, relevantArea);
        }
        
        public DemonstratesAttribute(Type typeOfThingBeingDemonstrated,
                                     RelevantArea relevantArea = RelevantArea.BodyOnly,
                                     string title = "",
                                     string description = "",
                                     Grouping grouping = Grouping.Default,
                                     IndexInGroup indexInGroup = IndexInGroup.Default,
                                     ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                     [CallerFilePath] string file = Default.CompilerServiceFile, 
                                     [CallerLineNumber] int line = Default.CompilerServiceLineNumber) 
            : this(typeOfThingBeingDemonstrated,
                   grouping,
                   indexInGroup,
                   file,
                   line,
                   title,
                   description,
                   relevantArea)
        {
            memberInfoOfThingBeingDemonstrated = typeOfThingBeingDemonstrated;
        }
        
        public DemonstratesAttribute(Type typeOfThingBeingDemonstrated, 
                                     string memberName, 
                                     RelevantArea relevantArea = RelevantArea.BodyOnly,
                                     string title = "",
                                     string description = "",
                                     Grouping grouping = Grouping.Default,
                                     IndexInGroup indexInGroup = IndexInGroup.Default,
                                     ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                     [CallerFilePath] string file = Default.CompilerServiceFile, 
                                     [CallerLineNumber] int line = Default.CompilerServiceLineNumber) 
            : this(typeOfThingBeingDemonstrated, 
                   grouping, 
                   indexInGroup, 
                   file, 
                   line, 
                   title, 
                   description, 
                   relevantArea)
        {
            Assert.IsTrue(TryGetNonOverloadedMember(typeOfThingBeingDemonstrated,
                                                    memberName,
                                                    out memberInfoOfThingBeingDemonstrated,
                                                    out string errorMsg),
                          $"{ErrorContext}: {errorMsg}");
        }
        
        public DemonstratesAttribute(Type typeOfThingBeingDemonstrated, 
                                     string memberName, 
                                     Type[] argumentTypes,
                                     RelevantArea relevantArea = RelevantArea.BodyOnly,
                                     string title = "",
                                     string description = "",
                                     Grouping grouping = Grouping.Default,
                                     IndexInGroup indexInGroup = IndexInGroup.Default,
                                     ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                     [CallerFilePath] string file = Default.CompilerServiceFile, 
                                     [CallerLineNumber] int line = Default.CompilerServiceLineNumber) 
            : this(typeOfThingBeingDemonstrated, 
                   grouping, 
                   indexInGroup, 
                   file, 
                   line, 
                   title, 
                   description, 
                   relevantArea)
        {
            Assert.IsTrue(TryGetOverloadedMember(typeOfThingBeingDemonstrated,
                                                 memberName,
                                                 argumentTypes,
                                                 out memberInfoOfThingBeingDemonstrated,
                                                 out string errorMsg),
                          $"{ErrorContext}: {errorMsg}");
        }

        public ThingDoingTheDocumenting GetThingDoingTheDocumenting(MemberInfo memberDoingTheDocumenting)
        {
            return new ThingDoingTheDocumenting(title,
                                                description,
                                                filePath,
                                                lineNumberRange,
                                                memberDoingTheDocumenting,
                                                grouping,
                                                indexInGroup);
        }

        public bool TryGetThingsBeingDocumented(out ThingBeingDocumented[] thingsBeingDocumented)
        {
            if (ThingBeingDocumented.TryCreate(memberInfoOfThingBeingDemonstrated,
                                               out thingsBeingDocumented,
                                               out string error))
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