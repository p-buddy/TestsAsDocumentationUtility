using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts;
using UnityEngine;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class MemberHelperTests
    {
        public class ExampleClass
        {
            public readonly struct ExampleNestedStruct
            {
                #region Constructors
                public ExampleNestedStruct(byte b) : this() { }
                public ExampleNestedStruct(byte b1, byte b2) : this() { }
                #endregion
                
                #region Fields
                public static byte ExamplePublicStaticField;
                private static byte ExamplePrivateStaticField;
                public readonly byte ExamplePublicField;
                private readonly byte ExamplePrivateField;
                #endregion Fields

                #region Properties
                public byte ExamplePublicProperty { get; }
                private byte ExamplePrivateProperty { get; }
                public static byte ExamplePublicStaticProperty { get; }
                private static byte ExamplePrivateStaticProperty { get; }
                #endregion
                
                #region Methods
                public void ExamplePublicMethod(int i, byte b) => throw new NotImplementedException();
                private void ExamplePrivateMethod(byte b) => throw new NotImplementedException();
                public static void ExamplePublicStaticMethod() => throw new NotImplementedException();
                private static void ExamplePrivateStaticMethod(int i, byte b) => throw new NotImplementedException();
                
                public void ExamplePublicGenericMethod<TGenericMethodType1, TGenericMethodType2>(
                    TGenericMethodType1 a,
                    TGenericMethodType2 b) =>
                    throw new NotImplementedException();
                #endregion Methods
            }
            
            public bool ExampleProperty { get; }
            public void ExampleMethod(int i, bool b)
            {
                throw new System.NotImplementedException();
            }

            public void ExampleGenericMethod<TGenericMethodType1, TGenericMethodType2>(TGenericMethodType1 a, TGenericMethodType2 b)
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void GetReadableNamesTest()
        {
            BindingFlags permissiveBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                                  BindingFlags.DeclaredOnly | BindingFlags.Instance;
            Type[] publicNestedTypes = GetType().GetNestedTypes();
            foreach (Type nestedType in publicNestedTypes)
            {
                MemberInfo[] members = nestedType.GetMembers(permissiveBindingFlags);
                List<string> readableNames = members.Select(MemberHelper.GetReadableName).ToList();
                readableNames.ForEach(Debug.Log);
            }

        }
    }
}