using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;


namespace pbuddy.TestsAsDocumentationUtility.EditorScripts.Declarations
{
    public static class Keywords
    {
        static MemberTypes AllMembersExcept(MemberTypes except) => MemberTypes.All ^ except;

        #region AccessModifiers
        private static readonly Keyword Public = new Keyword("public", MemberTypes.All);
        private static readonly Keyword Private = new Keyword("private", AllMembersExcept(MemberTypes.TypeInfo));
        private static readonly Keyword Internal = new Keyword("internal", MemberTypes.All);
        private static readonly Keyword Protected = new Keyword("protected", MemberTypes.All);
        #endregion AccessModifiers

        #region Type Declarations
        private static readonly Keyword Enum = new Keyword("enum", MemberTypes.TypeInfo);
        private static readonly Keyword Struct = new Keyword("struct", MemberTypes.TypeInfo);
        private static readonly Keyword Class = new Keyword("class", MemberTypes.TypeInfo);
        private static readonly Keyword Interface = new Keyword("interface", MemberTypes.TypeInfo);
        private static readonly Keyword Delegate = new Keyword("delegate", MemberTypes.All); // need to check
        private static readonly Keyword Event = new Keyword("event", MemberTypes.All); // need to check
        #endregion Type Declarations

        private static readonly Keyword ReadOnly = new Keyword("readonly", MemberTypes.Field);
        private static readonly Keyword Partial = new Keyword("partial", MemberTypes.TypeInfo | MemberTypes.Method);
        private static readonly Keyword Static = new Keyword("static", MemberTypes.All);
        private static readonly Keyword Abstract = new Keyword("abstract", MemberTypes.All);
        private static readonly Keyword Virtual = new Keyword("virtual", MemberTypes.All);
        private static readonly Keyword Override = new Keyword("override", MemberTypes.All);
        private static readonly Keyword Sealed = new Keyword("sealed", MemberTypes.TypeInfo);
        private static readonly Keyword Const = new Keyword("const", MemberTypes.All);
        private static readonly Keyword Fixed = new Keyword("const", MemberTypes.All);

        private static readonly Keyword Implicit = new Keyword("void", MemberTypes.Method);
        private static readonly Keyword Explicit = new Keyword("void", MemberTypes.Method);
        private static readonly Keyword Operator = new Keyword("void", MemberTypes.Method);

        private static readonly Keyword Void = new Keyword("void", MemberTypes.All);

        public static readonly Dictionary<Type, string> BuiltInTypes = new Dictionary<Type, string>
        {
            { typeof(Boolean), "bool" },
            { typeof(Byte), "byte" },
            { typeof(SByte), "sbyte" },
            { typeof(Int16), "short" },
            { typeof(UInt16), "ushort" },
            { typeof(Int32), "int" },
            { typeof(UInt32), "uint" },
            { typeof(Int64), "long" },
            { typeof(UInt64), "ulong" },
            { typeof(Double), "double" },
            { typeof(Single), "float" },
            { typeof(Decimal), "decimal" },
            { typeof(String), "string" },
            { typeof(Char), "char" },
            { typeof(Object), "object" }
        };

        private const string MatchAnyWhiteSpaceCharacter = "\\s";
        private const string OneOrMoreTimes = "+";
        
        public static string GetDeclarationRegex(MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    return GetDeclarationRegex(memberInfo as Type);
                case MemberTypes.Method:
                    return GetDeclarationRegex(memberInfo as MethodInfo);
                case MemberTypes.Field:
                    return GetDeclarationRegex(memberInfo as FieldInfo);
                case MemberTypes.Property:
                    return GetDeclarationRegex(memberInfo as PropertyInfo);
                case MemberTypes.Constructor:
                    return GetDeclarationRegex(memberInfo as ConstructorInfo);
                default:
                    return null;
            }
        }

        private static string GetDeclarationRegex(Type type)
        {
            string typeString = default;
            typeString = type.IsClass ? Class.Label : typeString;
            typeString = type.IsInterface ? Interface.Label : typeString;
            typeString = type.IsValueType ? Struct.Label : typeString;
            typeString = type.IsEnum ? Enum.Label : typeString;
            
            Assert.IsNotNull(typeString);

            string nextChar = type.IsGenericType ? "<" : "";
            return $"{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}{typeString}" + 
                   $"{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}{type.Name}" + 
                   $"[{nextChar}{MatchAnyWhiteSpaceCharacter}]{OneOrMoreTimes}";
        }
        
        public static string GetDeclarationRegex(MethodInfo methodInfo)
        {
            Type returnType = methodInfo.ReturnType;
            string returnTypeString = returnType == typeof(Void)
                ? Void.Label
                : BuiltInTypes.ContainsKey(returnType)
                    ? BuiltInTypes[returnType]
                    : returnType.Name;
            string nextChar = methodInfo.IsGenericMethod ? "<" : "(";
            return $"[{MatchAnyWhiteSpaceCharacter}\\.]{returnTypeString}" + 
                   $"{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}{methodInfo.Name}" + 
                   $"[{nextChar}{MatchAnyWhiteSpaceCharacter}]{OneOrMoreTimes}";
        }
        
        public static string GetDeclarationRegex(FieldInfo fieldInfo)
        {
            Type returnType = fieldInfo.FieldType;
            string returnTypeString = BuiltInTypes.ContainsKey(returnType)
                    ? BuiltInTypes[returnType]
                    : returnType.Name;
            return $"[{MatchAnyWhiteSpaceCharacter}\\.]{returnTypeString}" + 
                   $"{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}{fieldInfo.Name}" + 
                   $"[;={MatchAnyWhiteSpaceCharacter}]{OneOrMoreTimes}";
        }
        
        public static string GetDeclarationRegex(PropertyInfo propertyInfo)
        {
            Type returnType = propertyInfo.PropertyType;
            string returnTypeString = BuiltInTypes.ContainsKey(returnType)
                ? BuiltInTypes[returnType]
                : returnType.Name;
            return $"[{MatchAnyWhiteSpaceCharacter}\\.]{returnTypeString}" + 
                   $"{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}{propertyInfo.Name}" + 
                   $"[{{={MatchAnyWhiteSpaceCharacter}]{OneOrMoreTimes}";
        }
        
        public static void GetDeclarationRegex(EventInfo eventInfo)
        {
            
        }

        
        public static string GetDeclarationRegex(ConstructorInfo constructorInfo)
        {
            // (?:^\s+|\b(?:|protected|private|public|internal)\b)\s+FileReaderTests
            // FileReaderTests = name
        }
        
        private static void AddIf<T>(this List<T> list, bool condition, T item)
        {
            if (condition)
            {
                list.Add(item);
            }
        }
        
        
        private static void AddNameWithAllNamespaceLevels(this List<String> list, Type type)
        {
            list.Add(type.Name);
            string fullName = type.FullName;
            if (fullName is null)
            {
                return;
            }
            
            string[] parts = fullName.Split('.');
        }
    }
}