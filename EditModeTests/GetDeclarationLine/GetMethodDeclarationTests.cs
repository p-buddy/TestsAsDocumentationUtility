using System.Collections.Generic;
using NUnit.Framework;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class GetMethodDeclarationTests : GetDeclarationTestsBase
    {
        [ImmediatelyAboveDeclaration]
        public int A ()
        {
            return default;
        }
        
        [ImmediatelyAboveDeclaration]
        public void B<T> ()
        {
        }
        
        [ImmediatelyAboveDeclaration]
        private GetStructDeclarationTests.DummyStructB C ()
        {
            return default;
        }
        
        [ImmediatelyAboveDeclaration]
        private pbuddy.TestsAsDocumentationUtility.EditModeTests.GetStructDeclarationTests.DummyStructB D()
        {
            return default;
        }
        
        [ImmediatelyAboveDeclaration]
        GetStructDeclarationTests.DummyStructB E(int x)
        {
            return default;
        }
        
        [ImmediatelyAboveDeclaration]
        private static pbuddy.TestsAsDocumentationUtility.EditModeTests.GetStructDeclarationTests.DummyStructB F(bool x)
        {
            return default;
        }
        
        [ImmediatelyAboveDeclaration]
        static GetStructDeclarationTests.DummyStructB G(GetStructDeclarationTests.DummyStructD x)
        {
            return default;
        }
        
        [ImmediatelyAboveDeclaration]
        static void H ()
        {
        }
        
        [ImmediatelyAboveDeclaration]
        static bool I<T> ()
        {
            return default;
        }
        
        [ImmediatelyAboveDeclaration]
        static (bool, bool) J<T> ()
        {
            return default;
        }
        
        [ImmediatelyAboveDeclaration]
        static (bool, bool) K<T> () where T : class
        {
            return default;
        }
        
        [ImmediatelyAboveDeclaration]
        static List<(List<int>, List<int>, List<List<int>>)> L<T> () where T : class
        {
            return default;
        }
        

        [Test]
        public override void TestCase()
        {
            TestSpecificMember(nameof(L));
            BaseTestCase();
        }
    }
}