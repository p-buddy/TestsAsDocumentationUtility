using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public abstract partial class TestsAsDocumentationBase
    {
        private static readonly Assembly EditorAssembly = typeof(DemonstratesAttribute).Assembly;
        private static readonly With<Attribute> With = With<Attribute>.This<DemonstratesAttribute>();
        private static readonly Without<Attribute> Without = Without<Attribute>.These<TestAttribute, UnityTestAttribute>();
        
        private static readonly Dictionary<AssemblyName, Assembly> RelevantAssemblyByName;
        private static readonly Dictionary<Assembly, AssemblyName[]> ReferencesByAssembly;
        
        /// <summary>
        /// Documentation snippets 
        /// </summary>
        private static readonly Dictionary<Assembly, DocumentationSnippet[]> AuxiliarySnippets;

        static TestsAsDocumentationBase()
        {
            RelevantAssemblyByName = new Dictionary<AssemblyName, Assembly>();
            foreach (Assembly dependentAssembly in EditorAssembly.GetDependentAssemblies())
            {
                RelevantAssemblyByName.Add(dependentAssembly.GetName(), dependentAssembly);
            }
            
            ReferencesByAssembly = new Dictionary<Assembly, AssemblyName[]>();
            AuxiliarySnippets = new Dictionary<Assembly, DocumentationSnippet[]>();
        }

        protected void GetGeneratedDocumentationEntries()
        {
            
        }
        
        [TearDown]
        public void CreateDocumentation()
        {
            MemberInfo currentTest = GetCurrentTest();
            Assembly assembly = Assembly.GetAssembly(GetType());
            
            BuildAssemblyMap(assembly);

            foreach (DemonstratesAttribute demonstratesAttribute in currentTest.GetCustomAttributes<DemonstratesAttribute>())
            {
                DocumentationSnippet snippet = demonstratesAttribute.GetSnippet(currentTest);
                DocumentationGroup group = new DocumentationGroup(snippet);
                TryAddAllAuxiliarySnippets(assembly, in group);
                group.GetSnippets();
                // each documentation snippet is a single copy
                // so can process additions and removals first, which will affect line count
                // then process line ranges for highlights
            }
            
            CurrentMarkups.Clear();
        }

        private MemberInfo GetCurrentTest()
        {
            MemberInfo[] matchingMembers = GetType().GetMember(TestContext.CurrentContext.Test.MethodName);
            Assert.AreEqual(matchingMembers.Length, 1);
            return matchingMembers[0];
        }

        private void BuildAssemblyMap(Assembly assembly)
        {
            if (ReferencesByAssembly.ContainsKey(assembly)) return;
            
            List<AssemblyName> references = new List<AssemblyName>();
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                if (!RelevantAssemblyByName.TryGetValue(reference, out Assembly referencedAssembly))
                {
                    continue;
                }
                    
                BuildAuxiliarySnippetMap(referencedAssembly);
                references.Add(reference);
            }
            ReferencesByAssembly.Add(assembly, references.ToArray());
        }

        private static void BuildAuxiliarySnippetMap(Assembly assembly)
        {
            if (AuxiliarySnippets.ContainsKey(assembly)) return;

            List<MemberInfo> members = assembly.GetAllMembersWithAndWithout(in With, in Without);

            int snippetCountEstimate = members.Count + (int)(members.Count * 0.1);
            List<DocumentationSnippet> snippets = new List<DocumentationSnippet>(snippetCountEstimate);
            foreach (MemberInfo member in members)
            {
                foreach (DemonstratesAttribute demonstrates in member.GetCustomAttributes<DemonstratesAttribute>())
                {
                    snippets.Add(demonstrates.GetSnippet(member));
                }
            }
            
            AuxiliarySnippets.Add(assembly, snippets.Distinct().ToArray());
        }

        private static void TryAddAllAuxiliarySnippets(Assembly assembly, in DocumentationGroup group)
        {
            
        }
    }
}