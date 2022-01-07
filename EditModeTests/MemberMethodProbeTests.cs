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

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class MemberMethodProbeTests
    {
        public class Generic<T0, T1, T2, T3>
        {
            private int callCount;
            
            void AssertOneAndOnlyOneMatchingMemberProbe(string memberName, Type[] descriptiveTypes)
            {
                Type genericType = typeof(Generic<,,,>);
                MemberMethodProbe[] matchingMemberProbes = genericType.GetMember(memberName)
                                                                      .Select(m => new MemberMethodProbe(m))
                                                                      .Where(probe => probe.TypesMatch(descriptiveTypes))
                                                                      .ToArray();
                Assert.AreEqual(matchingMemberProbes.Length, 1);
                callCount++;
            }
            
            public void Method0<M0>(T0 a, M0 b, T1 c)
            {
                Type[] descriptiveTypes = {typeof(GenericTypeArgument0), typeof(GenericMethodArgument0), typeof(GenericTypeArgument1)};
                AssertOneAndOnlyOneMatchingMemberProbe(nameof(Method0), descriptiveTypes);
            }
            
            public void Method1(T3 a, int b)
            {
                Type[] descriptiveTypes = {typeof(GenericTypeArgument3), typeof(int)};
                AssertOneAndOnlyOneMatchingMemberProbe(nameof(Method1), descriptiveTypes);
            }
            
            public void Method2(T2 a, T3 b)
            {
                Type[] descriptiveTypes = {typeof(GenericTypeArgument2), typeof(GenericTypeArgument3)};
                AssertOneAndOnlyOneMatchingMemberProbe(nameof(Method2), descriptiveTypes);
            }
            
            public void Method2(T3 a, T2 b)
            {
                Type[] descriptiveTypes = {typeof(GenericTypeArgument3), typeof(GenericTypeArgument2)};
                AssertOneAndOnlyOneMatchingMemberProbe(nameof(Method2), descriptiveTypes);
            }
            
            public void Method2<M0, M1>(M0 a, M1 b)
            {
                Type[] descriptiveTypes = {typeof(GenericMethodArgument0), typeof(GenericMethodArgument1)};
                AssertOneAndOnlyOneMatchingMemberProbe(nameof(Method2), descriptiveTypes);
            }
            
            public void Method2<M0, M1>(M1 a, M0 b)
            {
                Type[] descriptiveTypes = {typeof(GenericMethodArgument1), typeof(GenericMethodArgument0)};
                AssertOneAndOnlyOneMatchingMemberProbe(nameof(Method2), descriptiveTypes);
            }

            public void Method2()
            {
                Type[] descriptiveTypes = Type.EmptyTypes;
                AssertOneAndOnlyOneMatchingMemberProbe(nameof(Method2), descriptiveTypes);
            }

            public void ConfirmAllMembersCalled(Type instantiatedType)
            {
                const string constructorName = ".ctor";
                const BindingFlags flags = BindingFlags.DeclaredOnly | 
                                           BindingFlags.Public | 
                                           BindingFlags.Instance;
                const string thisMethodName = nameof(ConfirmAllMembersCalled);
                const string assertMethodName = nameof(AssertOneAndOnlyOneMatchingMemberProbe);
                string[] excludedNames = { constructorName, thisMethodName, assertMethodName };
                var members = instantiatedType.GetMembers(flags).Where(m => !excludedNames.Contains(m.Name)).ToArray();
                Assert.AreEqual(members.Length,
                                callCount,
                                $"Number of members ({members.Length}) does not match call count ({callCount})");
            }
        }

        [Test]
        public void TestMemberMethods()
        {
            var instantiatedClass = new Generic<int, bool, byte, char>();
            
            instantiatedClass.Method0<string>(default, default, default);
            instantiatedClass.Method1(default, default);
            instantiatedClass.Method2((byte)default, default);
            instantiatedClass.Method2((char)default, default);
            instantiatedClass.Method2<char, string>((char)default, default);
            instantiatedClass.Method2<string, char>((char)default, default);
            instantiatedClass.Method2();
            
            instantiatedClass.ConfirmAllMembersCalled(instantiatedClass.GetType());
        }
    }
}