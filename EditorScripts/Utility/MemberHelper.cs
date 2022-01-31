using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine.Assertions;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class MemberHelper
    {
        private const BindingFlags PermissiveFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | 
                                                     BindingFlags.DeclaredOnly | BindingFlags.NonPublic;
        private static string OpenTag(string tagName) => $"|{tagName}-->";
        private static string CloseTag(string tagName) => $"<--{tagName}|";
        private static string InsertIntoTag(string tag, string name) => $"{OpenTag(tag)}{name}{CloseTag(tag)}";
        
        private const string MemberTagName = "Member";
        private const string AssemblyQualifiedTagName = "AssemblyName";
        private const string ConstructorTagName = "Constructor";
        private const string ArgumentSeparator = "|,|";
        
        private static readonly (string Open, string Close) AssemblyNameTag = (OpenTag(AssemblyQualifiedTagName), CloseTag(AssemblyQualifiedTagName));
        private static readonly (string Open, string Close) ConstructorTag = (OpenTag(ConstructorTagName), CloseTag(ConstructorTagName));
        private static readonly (string Open, string Close) MemberTag = (OpenTag(MemberTagName), CloseTag(MemberTagName));
        private static string MemberSegment(string member) => InsertIntoTag(MemberTagName, member);
        private static string AssemblyQualifiedSegment(string name) => InsertIntoTag(AssemblyQualifiedTagName, name);
        private static string ConstructArgsSegment(string text) => InsertIntoTag(ConstructorTagName, text);

        public static string GetReadableName(this MemberInfo memberInfo)
        {
            const string comma = ", ";
            static string GetTypeName(Type type) => type.Name;
            static string Combine(IEnumerable<string> pieces) => String.Join(comma, pieces);
            static string Wrap(string text, string open, string close) => $"{open}{text}{close}";
            static string GetFunctionParametersString(MethodBase method)
            {
                string[] parameters = method.GetParameters().Select(p => p.ParameterType.Name).ToArray();
                return parameters.IsNullOrEmpty() ? "()" : Wrap(Combine(parameters), "(", ")");
            }
            
            static string GetGenericArgumentsString(MemberInfo memberInfo)
            {
                return TryGetGenericArguments(memberInfo, out Type[] args) && !args.IsNullOrEmpty()
                    ? Wrap(Combine(args.Select(GetTypeName)), "<", ">")
                    : string.Empty;
            }
            
            

            switch (memberInfo.MemberType)
            {
                case MemberTypes.TypeInfo:
                    Type type = memberInfo as Type;
                    Assert.IsNotNull(type);
                    return $"{type.GetNonGenericName()}{GetGenericArgumentsString(type)}";
                case MemberTypes.NestedType:
                    Type nestedType = memberInfo as Type;
                    Assert.IsNotNull(nestedType);
                    return $"{GetReadableName(nestedType.DeclaringType)}+{nestedType.Name}{GetGenericArgumentsString(nestedType)}";
                case MemberTypes.Property:
                    PropertyInfo property = memberInfo as PropertyInfo;
                    Assert.IsNotNull(property);
                    return $"{GetReadableName(property.DeclaringType)}.{property.Name}";
                case MemberTypes.Field:
                    FieldInfo field = memberInfo as FieldInfo;
                    Assert.IsNotNull(field);
                    return $"{GetReadableName(field.DeclaringType)}.{field.Name}";
                case MemberTypes.Method:
                    MethodInfo method = memberInfo as MethodInfo;
                    Assert.IsNotNull(method);
                    return GetReadableName(method.DeclaringType) +
                           $".{method.Name}" +
                           GetGenericArgumentsString(method) +
                           GetFunctionParametersString(method);
                case MemberTypes.Constructor:
                    ConstructorInfo constructorInfo = memberInfo as ConstructorInfo;
                    Assert.IsNotNull(constructorInfo);
                    return GetReadableName(constructorInfo.DeclaringType) + GetFunctionParametersString(constructorInfo);
                default:
                    return "ERROR";
            }
        }
        
        public static string GetTypeRecoverableName(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.TypeInfo:
                    Type type = memberInfo as Type;
                    Assert.IsNotNull(type);
                    return AssemblyQualifiedSegment(type.AssemblyQualifiedName);
                case MemberTypes.Field:
                    FieldInfo field = memberInfo as FieldInfo;
                    Assert.IsNotNull(field);
                    return $"{GetTypeRecoverableName(field.DeclaringType)}{MemberSegment(field.Name)}";
                case MemberTypes.Property:
                    PropertyInfo property = memberInfo as PropertyInfo;
                    Assert.IsNotNull(property);
                    return $"{GetTypeRecoverableName(property.DeclaringType)}{MemberSegment(property.Name)}";
                case MemberTypes.Method:
                    static string GetParametersText(Type[] functionArguments)
                        => functionArguments == null || functionArguments.Length == 0
                            ? ""
                            : String.Join(ArgumentSeparator, functionArguments.Select(GetParameterTypeAssemblyName));
                    MethodInfo method = memberInfo as MethodInfo;
                    Assert.IsNotNull(method);
                    Type[] functionArguments = method.GetParameters().Select(p => p.ParameterType).ToArray();
                    return $"{GetTypeRecoverableName(method.DeclaringType)}" + 
                           MemberSegment($"{method.Name}({GetParametersText(functionArguments)})");
                case MemberTypes.NestedType:
                    Type nestedType = memberInfo as Type;
                    Assert.IsNotNull(nestedType);
                    return AssemblyQualifiedSegment(nestedType.AssemblyQualifiedName);
                case MemberTypes.Constructor:
                    ConstructorInfo constructorInfo = memberInfo as ConstructorInfo;
                    Assert.IsNotNull(constructorInfo);
                    string constructorArguments = String.Join(ArgumentSeparator,
                                                              constructorInfo.GetParameters()
                                                                             .Select(p => p.ParameterType)
                                                                             .Select(GetParameterTypeAssemblyName));
                    return $"{GetTypeRecoverableName(constructorInfo.DeclaringType)}{ConstructArgsSegment(constructorArguments)}";
                default:
                    return "ERROR";
            }
        }
        
        public static bool TryGetMemberFromTypeRecoverableName(this string text, out MemberInfo memberInfo)
        {
            string typeName = GetTextWithinTag(AssemblyNameTag, text);
            Type type = Type.GetType(typeName);
            if (type == null)
            {
                memberInfo = default;
                return false;
            }

            bool textIsTypeOnly = !(text.Contains(ConstructorTag.Open) || text.Contains(MemberTag.Open));
            if (textIsTypeOnly)
            {
                memberInfo = type;
                return true;
            }

            if (TryGetMemberFromText(text, type, out ConstructorInfo constructorInfo))
            {
                memberInfo = constructorInfo;
                return memberInfo != null;
            }

            if (TryGetMemberFromText(text, type, out PropertyInfo propertyInfo))
            {
                memberInfo = propertyInfo;
                return memberInfo != null;
            }

            if (TryGetMemberFromText(text, type, out MethodInfo methodInfo))
            {
                memberInfo = methodInfo;
                return memberInfo != null;
            }
            
            if (TryGetMemberFromText(text, type, out FieldInfo fieldInfo))
            {
                memberInfo = fieldInfo;
                return memberInfo != null;
            }

            memberInfo = default;
            return false;
        }

        public static string GetNonGenericName(this Type type, bool fullName = false)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }
            string name = fullName ? type.FullName : type.Name;
            Assert.IsNotNull(name);
            return name.Substring(0, name.LastIndexOf("`", StringComparison.Ordinal));
        }
        
        private static string GetReadableTypeName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.FullName;
            }
            
            StringBuilder sb = new StringBuilder();
            
            string AppendGenericTypeArgument(string aggregate, Type genericTypeArgument)
            {
                return aggregate + (aggregate == "<" ? "" : ",") + GetReadableTypeName(genericTypeArgument);
            }

            sb.Append(type.FullName.Substring(0, type.FullName.LastIndexOf("`", StringComparison.Ordinal)));
            sb.Append(type.GetGenericArguments().Aggregate("<", AppendGenericTypeArgument));
            sb.Append(">");

            return sb.ToString();
        }

        private static bool TryGetMemberFromText(this string text, Type declaringType, out ConstructorInfo constructorInfo)
        {
            if (!text.Contains(ConstructorTag.Open))
            {
                constructorInfo = default;
                return false;
            }
            
            string[] constructorArgNames = GetTextWithinTag(ConstructorTag, text).SplitArguments();
            bool ArgumentsMatch(ConstructorInfo methodInfo)
            {
                ParameterInfo[] parameters = methodInfo.GetParameters();
                return constructorArgNames.Length == 0 && parameters.Length == 0 ||
                       constructorArgNames.SequenceEqual(parameters.Select(p => p.ParameterType)
                                                                   .Select(GetParameterTypeAssemblyName));
            }

            ConstructorInfo[] constructorInfos = declaringType.GetConstructors(PermissiveFlags).Where(ArgumentsMatch).ToArray();
            constructorInfo = constructorInfos.Length == 1 ? constructorInfos[0] : default;
            return constructorInfos.Length == 1;
        }

        private static bool TryGetMemberFromText(this string text, Type declaringType, out FieldInfo fieldInfo)
        {
            if (!text.Contains(MemberTag.Open))
            {
                fieldInfo = default;
                return false;
            }
            
            string memberString = GetTextWithinTag(MemberTag, text);
            if (memberString.Contains("("))
            {
                fieldInfo = default;
                return false;
            }
            
            fieldInfo = declaringType.GetField(memberString, PermissiveFlags);
            return fieldInfo != null;
        }
        
        private static bool TryGetMemberFromText(this string text, Type declaringType, out PropertyInfo propertyInfo)
        {
            if (!text.Contains(MemberTag.Open))
            {
                propertyInfo = default;
                return false;
            }
            
            string memberString = GetTextWithinTag(MemberTag, text);
            if (memberString.Contains("("))
            {
                propertyInfo = default;
                return false;
            }
            
            propertyInfo = declaringType.GetProperty(memberString, PermissiveFlags);
            return propertyInfo != null;
        }

        private static bool TryGetMemberFromText(this string text, Type declaringType, out MethodInfo methodInfo)
        {
            if (!text.Contains(MemberTag.Open))
            {
                methodInfo = default;
                return false;
            }
            
            string memberString = GetTextWithinTag(MemberTag, text);
            if (!memberString.Contains("("))
            {
                methodInfo = default;
                return false;
            }
            
            (string open, string close) angleBracket = ("<", ">");
            (string Open, string Close) parenthesis = ("(", ")");
            
            int angleBracketIndex = memberString.IndexOf(angleBracket.open, StringComparison.Ordinal);
            string methodName = angleBracketIndex >= 0
                ? memberString.Substring(0, memberString.IndexOf(angleBracket.open, StringComparison.Ordinal))
                : memberString.Substring(0, memberString.IndexOf(parenthesis.Open, StringComparison.Ordinal));
            string[] argumentNames = GetTextWithinTag(parenthesis, memberString).SplitArguments();
            bool NameAndArgumentsMatch(MethodInfo method)
            {
                bool namesMatch = method.Name == methodName;
                if (!namesMatch)
                {
                    return false;
                }
                ParameterInfo[] parameters = method.GetParameters();
                return argumentNames.Length == 0 && parameters.Length == 0 ||
                       argumentNames.SequenceEqual(parameters.Select(p => p.ParameterType)
                                                             .Select(GetParameterTypeAssemblyName));
            }

            MethodInfo[] matchingMethods = declaringType.GetMethods(PermissiveFlags).Where(NameAndArgumentsMatch).ToArray();
            methodInfo = matchingMethods.Length == 1 ? matchingMethods[0] : default;
            return matchingMethods.Length == 1;
        }

        private static string GetTextWithinTag((string Open, string Close) tag, string text)
        {
            int start = text.IndexOf(tag.Open, StringComparison.Ordinal) + tag.Open.Length;
            int end = text.IndexOf(tag.Close, StringComparison.Ordinal);
            return text.Substring(start, end - start);
        }

        private static bool TryGetGenericArguments(MemberInfo memberInfo, out Type[] genericArguments)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    Type type = memberInfo as Type;
                    Assert.IsNotNull(type);
                    genericArguments = type.IsGenericType ? type.GetGenericArguments() : null;
                    return type.IsGenericType;
                case MemberTypes.Method:
                    MethodInfo method = memberInfo as MethodInfo;
                    Assert.IsNotNull(method);
                    genericArguments = method.IsGenericMethod ? method.GetGenericArguments() : null;
                    return method.IsGenericMethod;
            }

            genericArguments = default;
            return false;
        }

        private static string[] SplitArguments(this string argumentString)
        {
            return argumentString.Split(new[] { ArgumentSeparator }, StringSplitOptions.RemoveEmptyEntries);
        }
        
        private static string GetParameterTypeAssemblyName(Type type) => type.AssemblyQualifiedName ?? type.Name;
        private static bool IsNullOrEmpty(this Array array) =>  array == null || array.Length == 0;
    }
}