using System;
using System.Reflection;
using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts.Declarations;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class CodeMatcherTests
    {
        public struct GenericA<T1> { }
        
        public struct GenericB<T1, T3, T2> { }


        public GenericA<int> NotNested(out int returnTypeGenericCount)
        {
            returnTypeGenericCount = 1;
            return default;
        }
        
        public GenericA<(int, int)> NotNestedWithTuple(out int returnTypeGenericCount)
        {
            returnTypeGenericCount = 1;
            return default;
        }

        public GenericA<GenericB<int, int, int>> SimpleNested(out int returnTypeGenericCount)
        {
            returnTypeGenericCount = 1 + 1;
            return default;
        }

        public GenericA<
            GenericB<
                GenericA<
                    GenericB<int, int, GenericA<int>>
                >,
                GenericA<
                    GenericA<(int, int)>
                >,
                GenericA<
                    GenericB<
                        GenericA<int>,
                        int,
                        int
                    >
                >
            >
        > 
            VeryNested(out int returnTypeGenericCount)
        {
            returnTypeGenericCount = 1 + 1 + 3 + 1 + 2 + 1 + 1; 
            return default;
        }

        [Test]
        public void GetGenericCountTests()
        {
            NotNested(out int expectedNotNested);
            NotNestedWithTuple(out int expectedNotNestedWithTuple);
            SimpleNested(out int expectedSimpleNested);
            VeryNested(out int expectedVeryNested);
            
            AssertExpectedGenericCount(expectedNotNested, nameof(NotNested));
            AssertExpectedGenericCount(expectedNotNestedWithTuple, nameof(NotNestedWithTuple));
            AssertExpectedGenericCount(expectedSimpleNested, nameof(SimpleNested));
            AssertExpectedGenericCount(expectedVeryNested, nameof(VeryNested));
        }

        private void AssertExpectedGenericCount(int expected, string name)
        {
            MethodInfo method = GetType().GetMethod(name);
            Assert.NotNull(method);
            Type returnType = method.ReturnType;
            int actual = CodeMatcher.GetGenericCount(returnType);
            Assert.AreEqual(expected, actual, $"Generic count mismatch for {name}");
        }
    }
}