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
            DescriptiveArgumentTypes = ConvertToDescriptiveArgumentTypes(genericTypeArguments, 
                                                                         genericMethodArguments,
                                                                         ArgumentTypes,
                                                                         out descriptiveTypeByGenericType);
            genericTypeCount = genericTypeArguments?.Length ?? 0 + genericMethodArguments?.Length ?? 0;
        }


        public FileInfo InvokeAndGetFileInfo(out object returnValue)
        {
            returnValue = Invoke();
            StackTrace stackFrame = new StackTrace(true);
            var frames = stackFrame.GetFrames();
            return new FileInfo(frames[0].GetFileName(), frames[0].GetFileLineNumber());
        }
        
        public object Invoke()
        {
            var genericTypeMap = new Dictionary<Type, Type>(genericTypeCount);
            Type nonGenericDeclaringType = MakeNonGenericDeclaringType(this, ref genericTypeMap);
            MethodInfo nonGenericMethod = MakeNonGenericMethod(this, nonGenericDeclaringType, ref genericTypeMap);
            object declaringTypeInstance = Activator.CreateInstance(nonGenericDeclaringType);
            object[] arguments = ArgumentTypes.Select(t => ConvertToMappedType(t, genericTypeMap))
                                              .Select(Activator.CreateInstance)
                                              .ToArray();

            MethodBody x = MethodInfo.GetMethodBody();

            string path = Path.Combine(Application.dataPath, "mylog");
            Profiler.logFile = path; //Also supports passing "myLog.raw"
            Profiler.enableBinaryLog = true;
            Profiler.enabled = true;
            Profiler.BeginSample("Documentation");
            object obj = nonGenericMethod.Invoke(declaringTypeInstance, arguments);
            Profiler.EndSample();
            Profiler.enabled = false;
            Profiler.logFile = "";
            
            StackTrace st1 = new StackTrace(new StackFrame(1, true));
            return obj;
        }

        private static Type MakeNonGenericDeclaringType(in MemberMethodProbe probe,
                                                        ref Dictionary<Type, Type> nonGenericTypeByGenericTypeMap)
        {
            if (probe.declaringType.IsGenericType)
            {
                Type[] supplementedTypes = new Type[probe.genericTypeArguments.Length];
                for (var index = 0; index < probe.genericTypeArguments.Length; index++)
                {
                    // need to get fancy with type restraints
                    Type nonGenericType = typeof(object);
                    nonGenericTypeByGenericTypeMap[probe.genericTypeArguments[index]] = nonGenericType;
                    supplementedTypes[index] = nonGenericType;
                }

                Type genericType = probe.declaringType.MakeGenericType(supplementedTypes);
                return genericType;
            }

            return probe.declaringType;
        }

        private static MethodInfo MakeNonGenericMethod(in MemberMethodProbe probe,
                                                       Type nonGenericDeclaringType,
                                                       ref Dictionary<Type, Type> nonGenericTypeByGenericTypeMap)
        {
            var typeMap = nonGenericTypeByGenericTypeMap;
            int methodIndex = GetMethodIndexOnDeclaringType(in probe);
            Assert.IsTrue(methodIndex >= 0,
                          $"Could not retrieve method index for method '{probe.MethodInfo.Name}' " + 
                          $"on {probe.declaringType.GetReadableTypeName(false)} type");
            
            Type[] convertedArguments = probe.ArgumentTypes.Select(t => ConvertToMappedType(t, typeMap)).ToArray();
            MethodInfo nonGenericMethod = nonGenericDeclaringType.GetMethods()[methodIndex];
            Type[] methodParameters = nonGenericMethod.GetParameters()
                                                      .Select(parameterInfo => parameterInfo.ParameterType)
                                                      .ToArray();

            Assert.IsTrue(methodParameters.SequenceEqual(convertedArguments),
                          $"Error when retrieving non generic method for method '{probe.MethodInfo.Name}' " + 
                          $"on {probe.declaringType.GetReadableTypeName(false)} type. " +
                          $"The arguments of the retrieved non generic method ([{String.Join<Type>(",", methodParameters)}]) " +
                          $"did not match the arguments of the original method ([{String.Join<Type>(",", convertedArguments)}])");

            if (probe.MethodInfo.IsGenericMethod)
            { ;
                Type[] nonGenericMethodArguments = new Type[probe.genericMethodArguments.Length];
                for (int i = 0; i < probe.genericMethodArguments.Length; i++)
                {
                    // need to get fancy with type restraints
                    Type nonGenericType = typeof(object);
                    nonGenericMethodArguments[i] = nonGenericType;
                    typeMap[probe.genericMethodArguments[i]] = nonGenericType;
                }

                return nonGenericMethod.MakeGenericMethod(nonGenericMethodArguments);
            }

            return nonGenericMethod;
        }

        private static Type ConvertToMappedType(Type type, Dictionary<Type, Type> typeMap)
        {
            if (typeMap.TryGetValue(type, out Type mappedType))
            {
                return mappedType;
            }

            return type;
        }

        private static int GetMethodIndexOnDeclaringType(in MemberMethodProbe probe)
        {
            var methods = probe.declaringType.GetMethods();
            for (var i = 0; i < methods.Length; i++)
            {
                MethodInfo method = methods[i];
                if (method.Name == probe.MethodInfo.Name)
                {
                    IEnumerable<Type> methodParameters = method.GetParameters().Select(pInfo => pInfo.ParameterType);
                    if (methodParameters.SequenceEqual(probe.ArgumentTypes))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

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