using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public abstract class GetDeclarationTestsBase
    {
        private BindingFlags permissiveBindingFlags = BindingFlags.Public |
                                                      BindingFlags.NonPublic |
                                                      BindingFlags.Static |
                                                      BindingFlags.DeclaredOnly |
                                                      BindingFlags.Instance;
        
        [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
        protected class ImmediatelyAboveDeclarationAttribute : Attribute
        {
            public int AttributeLineNumber { get; }
            public int ExpectedDeclarationLine { get;}
            public string FileName { get; }
            
            public ImmediatelyAboveDeclarationAttribute([CallerLineNumber] int attributeLineNumberFilledInByCompiler = default, 
                                                        [CallerFilePath] string fileNameFilledInByCompiler = "")
            {
                AttributeLineNumber = attributeLineNumberFilledInByCompiler;
                ExpectedDeclarationLine = attributeLineNumberFilledInByCompiler + 1;
                FileName = fileNameFilledInByCompiler;
            }
        }

        protected void BaseTestCase()
        {
            
            Type type = GetType();
            MemberInfo[] members = type.GetMembers(permissiveBindingFlags);
            foreach (MemberInfo member in members)
            {
                TestSpecificMember(member);
            }
        }

        protected void TestSpecificMember(string memberName)
        {
            Type type = GetType();
            MemberInfo[] members = type.GetMember(memberName, permissiveBindingFlags);
            Assert.AreEqual(members.Length, 1);
            MemberInfo member = members[0];
            TestSpecificMember(member);
        }
        
        private void TestSpecificMember(MemberInfo member)
        {
            var attribute = member.GetCustomAttribute<ImmediatelyAboveDeclarationAttribute>();
            if (attribute != null)
            {
                int actualDeclarationLine = FileParser.FindDeclarationStartLine(member, attribute.FileName, 1);
                Assert.AreEqual(attribute.ExpectedDeclarationLine,
                                actualDeclarationLine,
                                $"Retrieved declaration for {member.Name} did not match expected");
            }
        }

        public abstract void TestCase();
    }
}