using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public abstract class TestsAsDocumentationBase
    {
        [Test]
        public void CreateDocumentation()
        {
            Type type = GetType();
            MethodInfo[] memberInfos = type.GetMethods();
            foreach (MethodInfo memberInfo in memberInfos)
            {
                IEnumerable<DemonstratesAttribute> documentsAttributes = memberInfo.GetCustomAttributes<DemonstratesAttribute>();
                foreach (DemonstratesAttribute documentation in documentsAttributes)
                {
                }
            }
        }
    }
}