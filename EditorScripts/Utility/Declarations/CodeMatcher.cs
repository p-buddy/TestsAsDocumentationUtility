using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;


namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class CodeMatcher
    {
        static MemberTypes AllMembersExcept(MemberTypes except) => MemberTypes.All ^ except;

        #region AccessModifiers
        private static readonly Keyword Public = new Keyword("public", MemberTypes.All);
        private static readonly Keyword Private = new Keyword("private", AllMembersExcept(MemberTypes.TypeInfo));
        private static readonly Keyword Internal = new Keyword("internal", MemberTypes.All);
        private static readonly Keyword Protected = new Keyword("protected", MemberTypes.All);
        private static readonly Keyword[] AccessModifiers = { Public, Private, Internal, Protected };
        private static readonly string MatchAnyAccessModifier = String.Join(Or, AccessModifiers.Select(modifier => modifier.Label));
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
        private const string ZeroOrMoreTimes = "*";
        private const string Period = "\\.";
        private const string Or = "|";
        private const string WordBoundary = "\b";
        private const string OpenGroup = "(";
        private const string CloseGroup = ")";
        private const string OpenNonCapturingGroup = OpenGroup + "?:";
        private const string AtStartOfString = "^";
        private const string EndOfString = "$";

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
        
        public static int GetGenericCount(Type type)
        {
            if (type.IsGenericType)
            {
                int sum = IsTuple(type) ? 0 : 1;
                Type[] genericArgs = type.GetGenericArguments();
                foreach (Type argType in genericArgs)
                {
                    sum += GetGenericCount(argType);
                }

                return sum;
            }

            return 0;
        }
        
        private static string GetTypeName(Type type) => BuiltInTypes.ContainsKey(type)
            ? BuiltInTypes[type]
            : type.GetNonGenericName();

        private static string GetTypeLabel(Type type)
        {
            if (type.IsClass)
            {
                return Class.Label;
            }

            if (type.IsInterface)
            {
                return Interface.Label;
            }

            if (type.IsValueType)
            {
                return Struct.Label;
            }

            if (type.IsEnum)
            {
                return Enum.Label;
            }

            return null;
        }

        private static string GetDeclarationRegex(Type type)
        {
            string typeString = GetTypeLabel(type);
            Assert.IsNotNull(typeString);

            string matchClose = $"{EndOfString}{Or}{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}";
            matchClose = type.IsGenericType ? $"{matchClose}{Or}<" : matchClose;
            return $"{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}{typeString}" + 
                   $"{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}{type.GetNonGenericName()}" + 
                   $"{OpenNonCapturingGroup}{matchClose}{CloseGroup}";
        }

        
        
        public static string GetDeclarationRegex(MethodInfo methodInfo)
        {
            Type returnType = methodInfo.ReturnType;
            string nextChar = methodInfo.IsGenericMethod ? "<" : "(";
            return $"{GetMatchReturnType(returnType)}" + 
                   $"{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}{methodInfo.Name}" + 
                   $"[{nextChar}{MatchAnyWhiteSpaceCharacter}]{OneOrMoreTimes}";
        }
        
        public static string GetDeclarationRegex(FieldInfo fieldInfo)
        {
            Type returnType = fieldInfo.FieldType;
            string returnTypeString = GetTypeName(returnType);
            return $"[{MatchAnyWhiteSpaceCharacter}{Period}]{returnTypeString}" + 
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
            // TODO 
        }


        public static string GetDeclarationRegex(ConstructorInfo constructorInfo)
        {
            string matchWhiteSpaceAtStart = $"{AtStartOfString}{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}";
            string matchAccessModifier = constructorInfo.IsStatic ? GetMatchWord(Static.Label) : GetMatchWord(MatchAnyAccessModifier);
            string matchOpener = $"{OpenNonCapturingGroup}{matchWhiteSpaceAtStart}{Or}{matchAccessModifier}{CloseGroup}";
            return $"{matchOpener}{MatchAnyWhiteSpaceCharacter}{OneOrMoreTimes}{constructorInfo.Name}";
        }
        
        private static string GetMatchReturnType(Type type)
        {
            if (IsTuple(type))
            {
                return "\\([^)]*\\)";
            }

            string typeName = type == typeof(void) ? Void.Label : GetTypeName(type);
            string matchTypeWithinAnyNameSpace = $"[{MatchAnyWhiteSpaceCharacter}{Period}]{typeName}";
            
            if (IsArray(type))
            {
                string matchArrayIdentifiers = $"\\[{MatchAnyWhiteSpaceCharacter}{ZeroOrMoreTimes}\\]";
                return $"{matchTypeWithinAnyNameSpace}{MatchAnyWhiteSpaceCharacter}{ZeroOrMoreTimes}{matchArrayIdentifiers}";
            }

            if (!type.IsGenericType)
            {
                return matchTypeWithinAnyNameSpace;
            }

            int genericCount = GetGenericCount(type);
            string closeGeneric = "[^>]*>";
            string matchGenericArgs = $"<{string.Concat(Enumerable.Repeat(closeGeneric, genericCount))}";
            return $"{matchTypeWithinAnyNameSpace}{MatchAnyWhiteSpaceCharacter}{ZeroOrMoreTimes}{matchGenericArgs}";
        }

        private static bool IsTuple(Type type) => type.Name.StartsWith("ValueTuple`");
        private static bool IsArray(Type type) => type.IsArray;

        private static string GetMatchWord(string word) =>
            $"{WordBoundary}{OpenNonCapturingGroup}{word}{CloseGroup}{WordBoundary}";
    }
}