using System.Reflection;
using NUnit.Framework;
using UnityEngine;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public abstract partial class TestsAsDocumentationBase
    {
        [TearDown]
        public void CreateDocumentation()
        {
            MemberInfo currentTest = GetCurrentTest();
            Assembly assembly = Assembly.GetAssembly(GetType());
        }

        private MemberInfo GetCurrentTest()
        {
            MemberInfo[] matchingMembers = GetType().GetMember(TestContext.CurrentContext.Test.MethodName);
            Assert.AreEqual(matchingMembers.Length, 1);
            return matchingMembers[0];
        }

        private void BuildAssemblyMap()
        {
            // get the referenced assemblies
            // determine which ones reference the editor scripts assembly (and thus could use the demonstrates attr)
            // of those, find all the demonstrates attributes
            // save those by member in a dictionary 
            // Kvp<Assembly, KVP<Member, DemonstratesAttribute>>
            
        }
    }
}