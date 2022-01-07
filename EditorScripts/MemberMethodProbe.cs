using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using pbuddy.TypeUtility.RuntimeScripts;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct MemberMethodProbe
    {
        public MemberInfo Member { get; }
        public MethodInfo MethodInfo { get; }
        public Type[] ArgumentTypes { get; }
        public int ArgumentCount { get; }
        public Type[] DescriptiveArgumentTypes { get; }
        
        private readonly Type[] genericTypeArguments;
        private readonly Type[] genericMethodArguments;
        private readonly int genericTypeCount;
        
        private readonly Dictionary<Type, Type> descriptiveTypeByGenericType;
        private readonly Type declaringType;
        public MemberMethodProbe(MemberInfo member)
        {
            Member = member;
            MethodInfo = member as MethodInfo;
            Assert.IsNotNull(MethodInfo, $"Member '{member.Name}' could not be converted to a method, and therefore couldn't be probed.");
            genericMethodArguments = MethodInfo.IsGenericMethod ? MethodInfo.GetGenericArguments() : null;
            
            declaringType = MethodInfo.DeclaringType;
            Assert.IsNotNull(declaringType);
            genericTypeArguments = declaringType.IsGenericType ? declaringType.GetGenericArguments() : null;

            ArgumentTypes = MethodInfo.GetParameters().Select(parameterInfo => parameterInfo.ParameterType).ToArray();
            ArgumentCount = ArgumentTypes.Length;
            DescriptiveArgumentTypes = ConvertToDescriptiveArgumentTypes(genericTypeArguments, 
                                                                         genericMethodArguments,
                                                                         ArgumentTypes,
                                                                         out descriptiveTypeByGenericType);
            genericTypeCount = genericTypeArguments?.Length ?? 0 + genericMethodArguments?.Length ?? 0;
        }       

        public bool TypesMatch(Type[] types) => types.SequenceEqual(DescriptiveArgumentTypes);

        private static Type[] ConvertToDescriptiveArgumentTypes(Type[] genericArgumentTypes, 
                                                                Type[] genericMethodTypes,
                                                                Type[] parameterTypes, 
                                                                out Dictionary<Type, Type> genericTypeMap)
        {
            Type[] convertedTypes = new Type[parameterTypes.Length];
            const int notFound = -1;
            int genericTypeCount = genericArgumentTypes?.Length ?? 0 + genericMethodTypes?.Length ?? 0;
            genericTypeMap = new Dictionary<Type, Type>(genericTypeCount);
            for (var index = 0; index < parameterTypes.Length; index++)
            {
                Type parameterType = parameterTypes[index];
                int genericTypeArgumentIndex = genericArgumentTypes != null
                    ? Array.IndexOf(genericArgumentTypes, parameterType)
                    : notFound;
                int genericMethodArgumentIndex = genericMethodTypes != null
                    ? Array.IndexOf(genericMethodTypes, parameterType)
                    : notFound;

                if (genericTypeArgumentIndex != notFound)
                {
                    genericTypeMap[parameterType] = GenericType.Arguments[genericTypeArgumentIndex];
                    convertedTypes[index] = GenericType.Arguments[genericTypeArgumentIndex];
                    continue;
                }

                if (genericMethodArgumentIndex != notFound)
                {
                    genericTypeMap[parameterType] = GenericMethod.Arguments[genericMethodArgumentIndex];
                    convertedTypes[index] = GenericMethod.Arguments[genericMethodArgumentIndex];
                    continue;
                }

                convertedTypes[index] = parameterType;
            }

            return convertedTypes;
        }

        private static bool IsGenericArgument(Type type) => typeof(IGenericArgument).IsAssignableFrom(type);
        private static bool IsGenericTypeArgument(Type type) => typeof(IGenericTypeArgument).IsAssignableFrom(type);
        private static bool IsGenericMethodArgument(Type type) => typeof(IGenericMethodArgument).IsAssignableFrom(type);
    }
}