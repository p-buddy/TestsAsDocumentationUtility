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
    public class DocumentsAttribute : Attribute
    { 
        private const string ErrorContext = "[" + nameof(DocumentsAttribute) + " ERROR]: ";
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        public TargetType TargetType { get; }
        public string FilePath { get; }
        public LineNumberRange LineNumberRange { get; }
        
        public Type Type { get; }
        private MemberInfo memberInfo;

        private DocumentsAttribute(Type type, TargetType targetType, string filePath, int lineNumber)
        {
            Type = type;
            TargetType = targetType;
            FilePath = filePath;
            LineNumberRange =  GetLineNumberRange(lineNumber, filePath);
        }
        
        public DocumentsAttribute(Type type,
                                  ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard,
                                  [CallerFilePath] string file = CompilerServicesDefaults.File,
                                  [CallerLineNumber] int line = CompilerServicesDefaults.LineNumber) : this(type, TargetType.ObjectType, file, line) { }
        
        public DocumentsAttribute(Type type,
                                  string memberName,
                                  ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard,
                                  [CallerFilePath] string file = CompilerServicesDefaults.File,
                                  [CallerLineNumber] int line = CompilerServicesDefaults.LineNumber) : this(type, TargetType.NonGenericMember, file, line)
        {
            Assert.IsTrue(TryGetNonOverloadedMember(type,
                                                    memberName,
                                                    out memberInfo,
                                                    out string errorMsg),
                          $"{ErrorContext}: {errorMsg}");
        }
        
        public DocumentsAttribute(Type type,
                                  string memberName,
                                  Type[] argumentTypes,
                                  ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard,
                                  [CallerFilePath] string file = CompilerServicesDefaults.File,
                                  [CallerLineNumber] int line = CompilerServicesDefaults.LineNumber) : this(type, TargetType.NonGenericMember, file, line)
        {
            
        }
        
        public DocumentsAttribute(Type type,
                                  string memberName,
                                  int genericArgumentCount,
                                  Type[] argumentTypes,
                                  ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard,
                                  [CallerFilePath] string file = CompilerServicesDefaults.File,
                                  [CallerLineNumber] int line = CompilerServicesDefaults.LineNumber) : this(type, TargetType.GenericMember, file, line)
        {
            
        }

        private static bool TryGetNonOverloadedMember(Type type, string memberName, out MemberInfo memberInfo, out string errorMsg)
        {
            MemberInfo[] members = type.GetMember(memberName, Flags);
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

            foreach (MemberInfo member in members.Where(member => member.Name == memberName))
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
                       $"Use an alternative {nameof(DocumentsAttribute)} constructor to better specify which member you are targeting.";
            return false;
        }

        
        private static bool TryGetOverloadedMember(Type type,
                                                   string memberName,
                                                   Type[] argumentTypes,
                                                   out MemberInfo memberInfo,
                                                   out string errorMsg)
        {
            errorMsg = default;
            memberInfo = default;
            return false;
        }

        private static LineNumberRange GetLineNumberRange(int attributeLineNumber, string filePath)
        {
            const char openCurlyBrace = '{';
            const char closeCurlyBrace = '}';
            
            string[] lines = File.ReadAllLines(filePath);
            int lineIndex = attributeLineNumber - 1;
            int openCurlyBraceCount = 0;
            
            int GetCharacterCount(char character) => lines[lineIndex].Count(c => c == character);
            int GetOpenCount() => GetCharacterCount(openCurlyBrace) - GetCharacterCount(closeCurlyBrace);
            void Step()
            {
                openCurlyBraceCount += GetOpenCount();
                lineIndex++;
            }
            
            // Step to method's opening bracket
            while (openCurlyBraceCount == 0)
            {
                Step();
            }
            int lineStart = lineIndex + 1;
            
            // Step until closing bracket
            while (openCurlyBraceCount > 0)
            {
                Step();
            }

            var lineEnd = lineIndex - 1;
            return new LineNumberRange(lineStart, lineEnd);
        }
    }
}