using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Newtonsoft.Json;
using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public abstract class TestsAsDocumentationBase
    {
        private const string ErrorContext = "[" + nameof(TestsAsDocumentationBase) + " ERROR]:";
        private static readonly string EditorScriptsAssembly = $"{nameof(pbuddy)}.{nameof(TestsAsDocumentationUtility)}.{nameof(EditorScripts)}";
        private static readonly string RuntimeScriptsAssembly = $"{nameof(pbuddy)}.{nameof(TestsAsDocumentationUtility)}.{nameof(RuntimeScripts)}";
        private const string DocumentationDirectory = "GeneratedDocumentation";
        private const string DocFxDirectoryName = "DocFx";
        private const string DocFxConfig = "docfx.json";
        private const string DefaultJson = "<default>";
        private const string AppTitleProperty = "_appTitle";
        private const string AppFooterProperty = "_appFooter";
        private const string BaseUrlProperty = "baseUrl";

        private readonly Dictionary<string, List<ThingBeingDocumented>> ThingsBeingDocumentedByFile =
            new Dictionary<string, List<ThingBeingDocumented>>();

        private readonly Dictionary<ThingBeingDocumented, DocumentationCollection> DocumentationCollectionMap = 
            new Dictionary<ThingBeingDocumented, DocumentationCollection>();

        /// <summary>
        /// To make things work nicely, you are required to implement this <see cref="CreateDocumentation"/>
        /// function on your classes deriving from <see cref="TestsAsDocumentationBase"/>.
        /// However, you can/should implement it EXACTLY like the code snippet below (don't forget the <see cref="TestAttribute"/> [Test Attribute]):
        /// <code>
        /// [Test]
        /// public override void CreateDocumentation() => CreateDocumentationInternal();
        /// </code>
        /// </summary>
        public abstract void CreateDocumentation();

        protected void InternalCreateDocumentation(bool enforceJsonHasBeenConfigured = true,
                                                   ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard,
                                                   [CallerFilePath] string filePassedByCompiler = "")
        {
            ThingsBeingDocumentedByFile.Clear();
            DocumentationCollectionMap.Clear();
            
            /*
            string documentationDirectory = GetDocumentationDirectory(filePassedByCompiler);
            PrepDocumentationDirectory(documentationDirectory);
            if (enforceJsonHasBeenConfigured)
            {
                ConfirmJsonIsModified(documentationDirectory);
            }
            */

            var demonstratesAttributes = new List<DemonstratesAttribute>();
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
                if (referencedAssemblies.FirstOrDefault(a => a.Name == EditorScriptsAssembly) != default)
                {
                    demonstratesAttributes.AddRange(assembly.GetCustomAttributes<DemonstratesAttribute>());
                }
            }

            foreach (DemonstratesAttribute demonstratesAttribute in demonstratesAttributes)
            {
                if (demonstratesAttribute.TryGetThingsBeingDocumented(out ThingBeingDocumented[] thingsBeingDocumented))
                {
                    foreach (ThingBeingDocumented thingBeingDocumented in thingsBeingDocumented)
                    {
                        string file = thingBeingDocumented.FileLocation;
                        if (ThingsBeingDocumentedByFile.TryGetValue(file, out List<ThingBeingDocumented> collection))
                        {
                            collection.Add(thingBeingDocumented);
                        }
                        else
                        {
                            ThingsBeingDocumentedByFile[file] = new List<ThingBeingDocumented>() { thingBeingDocumented };
                        }
                    }
                    
                }
            }
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
    }
}