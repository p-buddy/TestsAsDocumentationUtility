using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts;
using pbuddy.TypeUtility.RuntimeScripts;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;
using Debug = UnityEngine.Debug;

namespace pbuddy.TypeUtility.EditModeTests
{
    public class MemberMethodProbeTests
    {
        public class Generic<T0, T1, T2, T3>
        {
            public Type[] Method0<M0>(T0 a, M0 b, T1 c)
            {
                return new[]
                {
                    typeof(GenericTypeArgument0),
                    typeof(GenericMethodArgument0),
                    typeof(GenericTypeArgument1)
                };
            }
            
            public Type[] Method1(T3 a, int b)
            {
                return new[]
                {
                    typeof(GenericTypeArgument3),
                    typeof(int),
                };
            }
            
            public Type[] Method2(T2 a, T3 b)
            {
                return new[]
                {
                    typeof(GenericTypeArgument2),
                    typeof(GenericTypeArgument3),
                };
            }
            
            public Type[] Method2(T3 a, T2 b)
            {
                return new[]
                {
                    typeof(GenericTypeArgument3),
                    typeof(GenericTypeArgument2),
                };
            }
            
            public Type[] Method2<M0, M1>(M0 a, M1 b)
            {
                return new[]
                {
                    typeof(GenericMethodArgument0),
                    typeof(GenericMethodArgument1),
                };
            }
            
            public Type[] Method2<M0, M1>(M1 a, M0 b)
            {
                return new[]
                {
                    typeof(GenericMethodArgument1),
                    typeof(GenericMethodArgument0),
                };
            }
            
            public Type[] Method3<M0>() => Type.EmptyTypes;
        }

        [Test]
        public void TestMemberMethods()
        {
            const string constructorName = ".ctor";
            Type testType = typeof(Generic<,,,>);
            BindingFlags flags = BindingFlags.DeclaredOnly |
                                 BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Instance;
            var members = testType.GetMembers(flags).Where(m => m.Name != constructorName);
            
            foreach (MemberInfo member in members)
            {
                MemberMethodProbe probe = new MemberMethodProbe(member);
                Type[] argumentTypes = probe.DescriptiveArgumentTypes;
                string functionName = $"{member.Name}({String.Join(",", argumentTypes.Select(t => t.Name))})";
                probe.InvokeAndGetFileInfo(out object returnValue);
                Type[] returnedTypes = returnValue as Type[];
                
                Assert.IsNotNull(returnedTypes, functionName);
                Assert.AreEqual(argumentTypes.Length, returnedTypes.Length, functionName);
                for (var index = 0; index < returnedTypes.Length; index++)
                {
                    Assert.AreEqual(returnedTypes[index], argumentTypes[index], functionName);
                }
            }
        }

        public class T<T0, T1> where T0 : IGenericArgument where T1 : struct 
        {
            public void Te(T0 inte)
            {
                
            }
            
            public void Tes(T1 st)
            {
                
            }
            
            public void Tes3(int i)
            {
                int y = i + i;
            }
        }
        
        [Test]
        public void Test()
        {
            Type baseType = typeof(T<,>);
            var parent = new T<IGenericArgument, int>();
            var method0 = parent.GetType().GetMember(nameof(T<IGenericArgument, int>.Te))[0] as MethodInfo;
            var method1 = parent.GetType().GetMember(nameof(T<IGenericArgument, int>.Tes))[0] as MethodInfo;
            var method2 = parent.GetType().GetMember(nameof(T<IGenericArgument, int>.Tes3))[0] as MethodInfo;


            method0.Invoke(parent, new object[]{null});
            method1.Invoke(parent, new object[]{null});
            method2.Invoke(parent, new object[]{null});

        }

        [Test]
        public void attrTest()
        {
            Add().GetCustomAttribute<SomeAttribute>();
        }

        public Type Add()
        {
            var type = typeof(SomeClass);

            var aName = new System.Reflection.AssemblyName("SomeNamespace");
            var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
            var mb = ab.DefineDynamicModule(aName.Name);
            var tb = mb.DefineType(type.Name + "Proxy", System.Reflection.TypeAttributes.Public, type);

            var attrCtorParams = new Type[] { typeof(int), typeof(string) };
            var attrCtorInfo = typeof(SomeAttribute).GetConstructor(attrCtorParams);
            var attrBuilder = new CustomAttributeBuilder(attrCtorInfo, new object[] { });
            tb.SetCustomAttribute(attrBuilder);

            var newType = tb.CreateType();
            var instance = (SomeClass)Activator.CreateInstance(newType);

            Assert.AreEqual("Test", instance.Value);
            var attr = (SomeAttribute)instance.GetType()
                                              .GetCustomAttributes(typeof(SomeAttribute), false)
                                              .SingleOrDefault();
            Assert.IsNotNull(attr);

            return newType;
        }
        
        public class SomeAttribute : Attribute
        {
            public SomeAttribute([CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
            {
                Debug.Log(line);
                Debug.Log(file);
            }

            public string Value { get; set; }
        }

        public class SomeClass
        {
            public string Value = "Test";
        }
    }
}