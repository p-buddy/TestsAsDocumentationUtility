using System;
using System.Collections.Generic;
using System.Reflection;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts.Declarations
{
    public static class Keywords
    {
        static MemberTypes AllMembersExcept(MemberTypes except) => MemberTypes.All ^ except;

        public static readonly Keyword Public = new Keyword("public", MemberTypes.All);
        public static readonly Keyword Private = new Keyword("private", AllMembersExcept(MemberTypes.TypeInfo));
        public static readonly Keyword Internal = new Keyword("internal", MemberTypes.All);
        public static readonly Keyword Protected = new Keyword("protected", MemberTypes.All);
        public static readonly Keyword ReadOnly = new Keyword("readonly", MemberTypes.Field);

        public static readonly Keyword Enum = new Keyword("enum", MemberTypes.TypeInfo);
        public static readonly Keyword Struct = new Keyword("struct", MemberTypes.TypeInfo);
        public static readonly Keyword Class = new Keyword("class", MemberTypes.TypeInfo);
        public static readonly Keyword Delegate = new Keyword("delegate", MemberTypes.All); // need to check
        public static readonly Keyword Event = new Keyword("event", MemberTypes.All); // need to check
        public static readonly Keyword Interface = new Keyword("interface", MemberTypes.TypeInfo);

        public static readonly Keyword Partial = new Keyword("partial", MemberTypes.TypeInfo | MemberTypes.Method);
        public static readonly Keyword Static = new Keyword("static", MemberTypes.All);
        public static readonly Keyword Abstract = new Keyword("abstract", MemberTypes.All);
        public static readonly Keyword Virtual = new Keyword("virtual", MemberTypes.All);
        public static readonly Keyword Override = new Keyword("override", MemberTypes.All);
        public static readonly Keyword Sealed = new Keyword("sealed", MemberTypes.TypeInfo);
        public static readonly Keyword Const = new Keyword("const", MemberTypes.All);
        public static readonly Keyword Fixed = new Keyword("const", MemberTypes.All);

        public static readonly Keyword Implicit = new Keyword("void", MemberTypes.Method);
        public static readonly Keyword Explicit = new Keyword("void", MemberTypes.Method);
        public static readonly Keyword Operator = new Keyword("void", MemberTypes.Method);

        public static readonly Keyword Void = new Keyword("void", MemberTypes.All);

        public static readonly Dictionary<Type, string> BuiltInTypes = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(sbyte), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(decimal), "decimal" },
            { typeof(string), "string" },
            { typeof(char), "char" },
            { typeof(object), "object" }
        };

        public static void GetModifiers(Type type,
                                        out List<Keyword> potentialModifiers,
                                        out List<Keyword> requiredModifiers)
        {
            potentialModifiers = new List<Keyword>();
            requiredModifiers = new List<Keyword>();
            
            if (type.IsNested)
            {
                potentialModifiers.AddIf(Private, type.IsNestedPrivate);
                requiredModifiers.AddIf(Public, type.IsNestedPublic);
                requiredModifiers.AddIf(Protected, type.IsNestedFamily || type.IsNestedFamORAssem);
                requiredModifiers.AddIf(Internal, type.IsNestedAssembly || type.IsNestedFamORAssem);
            }
            else
            {
                potentialModifiers.AddIf(Internal, !type.IsVisible);
                requiredModifiers.AddIf(Public, type.IsPublic);
            }
            
            if (type.IsClass)
            {
                requiredModifiers.AddIf(Static, type.IsAbstract && type.IsSealed);
                requiredModifiers.AddIf(Abstract, type.IsAbstract && !type.IsSealed);
                requiredModifiers.AddIf(Sealed, !type.IsAbstract && type.IsSealed);
            }
            
            // Delegate??
        }

        public static void GetModifiers(MethodInfo methodInfo,
                                        out List<Keyword> potentialModifiers,
                                        out List<Keyword> requiredModifiers)
        {
            
        }
        
        public static void GetModifiers(FieldInfo fieldInfo,
                                        out List<Keyword> potentialModifiers,
                                        out List<Keyword> requiredModifiers)
        {
            
        }
        
        public static void GetModifiers(PropertyInfo propertyInfo,
                                        out List<Keyword> potentialModifiers,
                                        out List<Keyword> requiredModifiers)
        {
            
        }
        
        public static void GetModifiers(EventInfo eventInfo,
                                        out List<Keyword> potentialModifiers,
                                        out List<Keyword> requiredModifiers)
        {
            
        }

        
        public static void GetModifiers(ConstructorInfo constructorInfo,
                                        out List<Keyword> potentialModifiers,
                                        out List<Keyword> requiredModifiers)
        {
        }
        
        private static void AddIf<T>(this List<T> list, T item, bool condition)
        {
            if (condition)
            {
                list.Add(item);
            }
        }
    }
}