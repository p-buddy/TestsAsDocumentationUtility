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
        private static readonly With<Attribute> WithDemonstratesAttr = With<Attribute>.This<DemonstratesAttribute>();
        private static readonly Without<Attribute> WithoutTestAttr = Without<Attribute>.These<TestAttribute, UnityTestAttribute>();
        
        private static readonly Dictionary<AssemblyName, Assembly> RelevantAssemblyByName;
        private static readonly Dictionary<Assembly, Assembly[]> ReferencesByAssembly;
        private static readonly Dictionary<Assembly, DocumentationSnippet[]> AuxiliarySnippetsByAssembly;

        static TestsAsDocumentationBase()
        {
            RelevantAssemblyByName = new Dictionary<AssemblyName, Assembly>();
            foreach (Assembly dependentAssembly in EditorAssembly.GetDependentAssemblies())
            {
                RelevantAssemblyByName.Add(dependentAssembly.GetName(), dependentAssembly);
            }
            
            ReferencesByAssembly = new Dictionary<Assembly, Assembly[]>();
            AuxiliarySnippetsByAssembly = new Dictionary<Assembly, DocumentationSnippet[]>();
        }
        
        [TearDown]
        public void CreateDocumentation()
        {
            IEnumerable<DocumentationEntry> entries = GetGeneratedDocumentationEntries();
            CurrentMarkups.Clear();
        }

        protected List<DocumentationEntry> GetGeneratedDocumentationEntries()
        {
            MemberInfo currentTest = GetCurrentTest();
            Assembly assembly = Assembly.GetAssembly(GetType());
            Assembly[] references = GetReferencedDemonstratingAssemblies(assembly);

            List<DocumentationSnippet> auxiliarySnippets = GetAuxiliarySnippets(new List<Assembly>(references){assembly});
            
            var testSnippets = currentTest.GetCustomAttributes<DemonstratesAttribute>()
                                          .Select(demonstration => demonstration.GetSnippet(currentTest))
                                          .ToList();
            
            return testSnippets.Select(GetEntry).ToList();

            #region Local Function(s)
            DocumentationEntry GetEntry(DocumentationSnippet snippet)
            {
                DocumentationGroup group = new DocumentationGroup(snippet);
                if (snippet.Group != Grouping.None)
                {
                    auxiliarySnippets.ForEach(aux => group.TryAddToGroup(in aux));
                }
                return new DocumentationEntry(in group, CurrentMarkups);
            }
            #endregion
        }

        private MemberInfo GetCurrentTest()
        {
            MemberInfo[] matchingMembers = GetType().GetMember(TestContext.CurrentContext.Test.MethodName);
            Assert.AreEqual(matchingMembers.Length, 1);
            return matchingMembers[0];
        }

        private Assembly[] GetReferencedDemonstratingAssemblies(Assembly assembly)
        {
            if (ReferencesByAssembly.TryGetValue(assembly, out Assembly[] references))
            {
                return references;
            }

            Assembly[] relevantReferences = assembly.GetReferencedAssemblies()
                                                                   .Where(name => RelevantAssemblyByName.ContainsKey(name))
                                                                   .Select(name => RelevantAssemblyByName[name])
                                                                   .ToArray();
            
            ReferencesByAssembly.Add(assembly, relevantReferences);
            return relevantReferences;
        }

        private List<DocumentationSnippet> GetAuxiliarySnippets(List<Assembly> assemblies)
        {
            List<DocumentationSnippet> snippets = new List<DocumentationSnippet>();

            assemblies.ForEach(assembly =>
            {
                if (!AuxiliarySnippetsByAssembly.TryGetValue(assembly, out DocumentationSnippet[] snips))
                {
                    snips = assembly.GetAllMembersWithAndWithout(in WithDemonstratesAttr, in WithoutTestAttr)
                                    .Select(ExtractSnippetsFromMembers)
                                    .SelectMany(Flatten)
                                    .ToArray();
                    AuxiliarySnippetsByAssembly[assembly] = snips;
                }
                snippets.AddRange(snips);
            });
            
            return snippets;
            
            #region Local Function(s)
            IEnumerable<DocumentationSnippet> ExtractSnippetsFromMembers(MemberInfo member) =>
                member.GetCustomAttributes<DemonstratesAttribute>()
                      .Select(demonstrates => demonstrates.GetSnippet(member));
            
            T Flatten<T>(T self) => self;
            #endregion Local Function(s)
        }
    }
}