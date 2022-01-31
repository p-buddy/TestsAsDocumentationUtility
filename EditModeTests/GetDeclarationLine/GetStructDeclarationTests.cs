using NUnit.Framework;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class GetStructDeclarationTests : GetDeclarationTestsBase
    {
        [ImmediatelyAboveDeclaration]
        private struct DummyStructA { }
        
        [ImmediatelyAboveDeclaration]
        public struct DummyStructB
        { }
        
        [ImmediatelyAboveDeclaration]
        protected struct DummyStructC { }
        
        [ImmediatelyAboveDeclaration]
        internal struct DummyStructD 
        { }
        
        [ImmediatelyAboveDeclaration]
        struct DummyStructE { }

        [ImmediatelyAboveDeclaration]
        struct DummyStructF<T1, T2> { }
        
        [ImmediatelyAboveDeclaration]
        struct DummyStructG <T1, T2> where T1 : class { }

        [Test]
        public override void TestCase()
        {
            BaseTestCase();
        }
    }
}