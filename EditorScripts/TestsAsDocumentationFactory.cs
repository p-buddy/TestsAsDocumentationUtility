using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Newtonsoft.Json;
using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;
using UnityEngine;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class TestsAsDocumentationFactory
    {
        private const string ErrorContext = "[" + nameof(TestsAsDocumentationFactory) + " ERROR]:";
        private const string DocumentationDirectory = "GeneratedDocumentation";
        private const string DocFxDirectoryName = "DocFx";
        private const string DocFxConfig = "docfx.json";
        private const string DefaultJson = "<default>";
        private const string AppTitleProperty = "_appTitle";
        private const string AppFooterProperty = "_appFooter";
        private const string BaseUrlProperty = "baseUrl";
        private const BindingFlags PermissiveBindingFlags = BindingFlags.Public |
                                                            BindingFlags.NonPublic |
                                                            BindingFlags.Static |
                                                            BindingFlags.DeclaredOnly |
                                                            BindingFlags.Instance;
        
        public static void CreateDocumentation(bool enforceJsonHasBeenConfigured = true,
                                                       ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard,
                                                       [CallerFilePath] string filePassedByCompiler = "")
        {

            /*
            string documentationDirectory = GetDocumentationDirectory(filePassedByCompiler);
            PrepDocumentationDirectory(documentationDirectory);
            if (enforceJsonHasBeenConfigured)
            {
                ConfirmJsonIsModified(documentationDirectory);
            }
            */
            Assembly editorAssembly = typeof(DemonstratesAttribute).Assembly;
            Assembly[] editorDependentAssemblies = editorAssembly.GetDependentAssemblies();
            List<MemberInfo> demonstratingMembers = editorDependentAssemblies.GetAllMembersWith<DemonstratesAttribute>();
            List<Documentation> documents = demonstratingMembers.CreateDocuments();

            var documentationBySubject = new Dictionary<MemberInfo, DocumentationCollection>();
            documents.ForEach(doc =>
            {
                if (documentationBySubject.TryGetValue(doc.MemberBeingDocumnted, out DocumentationCollection collection))
                {
                    collection.Add(doc);
                }
                else
                {
                    documentationBySubject[doc.MemberBeingDocumnted] = new DocumentationCollection(in doc);
                }
                Debug.Log("hi");
                Debug.Log(doc.GenerateMarkdown());
            });
            
            
            return;

            Assembly runtimeAssembly = typeof(DemonstratedByAttribute).Assembly;
            Assembly[] runtimeDependentAssemblies = runtimeAssembly.GetDependentAssemblies();
            var demonstratedByAttributes = runtimeDependentAssemblies.GetAllAttributes<DemonstratedByAttribute>();
        }

        private static void PrepDocumentationDirectory(string documentationDirectory)
        {
            string docFxDirectory = GetDoxFxDirectory();
            Directory.CreateDirectory(documentationDirectory);
            CopyOverFilesThatDontExist(docFxDirectory, documentationDirectory);
        }

        private static void ConfirmJsonIsModified(string documentationDirectory)
        {
            using StreamReader fileStream = new StreamReader(Path.Combine(documentationDirectory, DocFxConfig));
            JsonTextReader reader = new JsonTextReader(fileStream);

            bool IsProperty(string propertyName) => reader.TokenType == JsonToken.PropertyName && (string)reader.Value == propertyName;
            string GetValueIfProperty(string propertyName) => IsProperty(propertyName) && reader.Read() ? (string)reader.Value : null;
            void AssertValidity(string value, string propertyName)
            {
                Assert.IsNotNull(value);
                Assert.AreNotEqual(DefaultJson,
                                   value,
                                   $"{ErrorContext} {propertyName} was left as the default. " + 
                                   "Please update it to ensure your documentation is as useful as possible.");
            }
            
            string appTitle = null;
            string appFooter = null;
            string baseUrl = null;
            while (reader.Read())
            {
                appTitle = GetValueIfProperty(AppTitleProperty) ?? appTitle;
                appFooter = GetValueIfProperty(AppFooterProperty) ?? appFooter;
                baseUrl = GetValueIfProperty(BaseUrlProperty) ?? baseUrl;
            }
            reader.Close();
            
            AssertValidity(appTitle, AppTitleProperty);
            AssertValidity(appFooter, AppFooterProperty);
            AssertValidity(baseUrl, BaseUrlProperty);
        }

        private static void CopyOverFilesThatDontExist(string source, string destination)
        {
            string[] existingFiles = Directory.GetFiles(destination);
            foreach (string file in Directory.GetFiles(source))
            {
                string fileName = Path.GetFileName(file);
                if (!existingFiles.Select(Path.GetFileName).Contains(fileName))
                {
                    File.Copy(file, Path.Combine(destination, fileName));
                }
            }
        }
        private static string GetDoxFxDirectory() => Path.Combine(GetEditorScriptsPath(), DocFxDirectoryName);
        private static string GetDocumentationDirectory(string testFile) =>
            Path.Combine(Directory.GetParent(Path.GetDirectoryName(testFile)).FullName, DocumentationDirectory);
        private static string GetEditorScriptsPath([CallerFilePath] string callingFile = "") => Path.GetDirectoryName(callingFile);

        private static Assembly[] GetDependentAssemblies(this Assembly assembly)
        {
            bool IsDependencyAssembly(AssemblyName assemblyRef) => assemblyRef.Name == assembly.GetName().Name;

            bool DoesReferenceDependency(Assembly otherAssembly)
            { 
                return otherAssembly.GetReferencedAssemblies().ToList().Any(IsDependencyAssembly);
            }
            
            return AppDomain.CurrentDomain.GetAssemblies()
                            .ToList()
                            .Where(DoesReferenceDependency)
                            .ToArray();
        }

        private static List<MemberInfo> GetAllMembersWith<TAttributeType>(this Assembly[] assemblies)
            where TAttributeType : Attribute
        {
            var members = new List<MemberInfo>();
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    members.AddIf(type.Has<TAttributeType>(), type);
                    members.AddRange(type.GetMembers(PermissiveBindingFlags).Where(member => member.Has<TAttributeType>()));
                }
            }

            return members;
        }
        
        private static List<TAttributeType> GetAllAttributes<TAttributeType>(this Assembly[] assemblies)
            where TAttributeType : Attribute
        {
            var attributes = new List<TAttributeType>();
            foreach (Assembly assembly in assemblies)
            {
                attributes.AddRange(assembly.GetCustomAttributes<TAttributeType>());
            }

            return attributes;
        }

        private static List<Documentation> CreateDocuments(this List<MemberInfo> memberInfos)
        {
            var documents = new List<Documentation>(memberInfos.Count);
            foreach (MemberInfo member in memberInfos)
            {
                foreach (DemonstratesAttribute demonstrates in member.GetCustomAttributes<DemonstratesAttribute>())
                { 
                    documents.Add(demonstrates.GetDocument(member));
                }
            }

            return documents;
        }
        
        private static void AddOrInsert<TKeyType, TListItemType>(this Dictionary<TKeyType, List<TListItemType>> map,
                                                                 TKeyType key,
                                                                 TListItemType item)
        {
            if (map.TryGetValue(key, out List<TListItemType> collection))
            {
                collection.Add(item);
                return;
            }

            map[key] = new List<TListItemType> { item };
        }
        
        
        
        private static void AddIf<TListItemType>(this List<TListItemType> list, bool condition, TListItemType item)
        {
            if (condition)
            {
                list.Add(item);
            }
        }

        private static bool Has<TAttributeType>(this MemberInfo memberInfo) where TAttributeType: Attribute =>
            memberInfo.GetCustomAttribute<TAttributeType>() != null;
    }
}