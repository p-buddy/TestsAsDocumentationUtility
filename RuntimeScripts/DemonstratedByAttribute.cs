using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace pbuddy.TestsAsDocumentationUtility.RuntimeScripts
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DemonstratedByAttribute : Attribute
    {
        private readonly string title;
        public bool Generated { get; }
        public int StartingLineNumber { get; }
        public string FileLocation { get; }
        public string[] DocumentationDetails { get; }

        /// <summary>
        /// Blank constructor (added by developer once and only once)
        /// </summary>
        /// <param name="guard"></param>
        /// <param name="filePassedByCompiler"></param>
        /// <param name="linePassedByCompiler"></param>
        public DemonstratedByAttribute(ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                       [CallerFilePath] string filePassedByCompiler = Default.CompilerServiceFile, 
                                       [CallerLineNumber] int linePassedByCompiler = Default.CompilerServiceLineNumber)
        {
            Generated = false;
            FileLocation = filePassedByCompiler;
            StartingLineNumber = linePassedByCompiler;
        }
        
        /// <summary>
        /// [NOT FOR MANUAL USE] Constructor for one-off documentation with no title nor descriptor 
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <param name="lineRange"></param>
        /// <param name="guard"></param>
        /// <param name="filePassedByCompiler"></param>
        /// <param name="linePassedByCompiler"></param>
        public DemonstratedByAttribute(string fileLocation, 
                                       string lineRange, 
                                       ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                       [CallerFilePath] string filePassedByCompiler = Default.CompilerServiceFile, 
                                       [CallerLineNumber] int linePassedByCompiler = Default.CompilerServiceLineNumber)
        {
            Generated = true;
            DocumentationDetails = new []{ fileLocation, lineRange };
            FileLocation = filePassedByCompiler;
            StartingLineNumber = linePassedByCompiler;
        }

        /// <summary>
        /// [NOT FOR MANUAL USE] Constructor for one-off documentation with title
        /// </summary>
        /// <param name="title"></param>
        /// <param name="fileLocation"></param>
        /// <param name="lineRange"></param>
        /// <param name="guard"></param>
        /// <param name="filePassedByCompiler"></param>
        /// <param name="linePassedByCompiler"></param>
        public DemonstratedByAttribute(string title, 
                                       string fileLocation, 
                                       string lineRange, 
                                       ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                       [CallerFilePath] string filePassedByCompiler = Default.CompilerServiceFile, 
                                       [CallerLineNumber] int linePassedByCompiler = Default.CompilerServiceLineNumber)
        {
            this.title = title;
            Generated = true;
            DocumentationDetails = new []{ title, fileLocation, lineRange };
            FileLocation = filePassedByCompiler;
            StartingLineNumber = linePassedByCompiler;
        }

        /// <summary>
        /// [NOT FOR MANUAL USE] Constructor for one-off documentation with title and description
        /// </summary>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="fileLocation"></param>
        /// <param name="lineRange"></param>
        /// <param name="guard"></param>
        /// <param name="filePassedByCompiler"></param>
        /// <param name="linePassedByCompiler"></param>
        public DemonstratedByAttribute(string title, 
                                       string description,
                                       string fileLocation, 
                                       string lineRange, 
                                       ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                       [CallerFilePath] string filePassedByCompiler = Default.CompilerServiceFile, 
                                       [CallerLineNumber] int linePassedByCompiler = Default.CompilerServiceLineNumber)
        {
            this.title = title;
            Generated = true;
            DocumentationDetails = new []{ title, fileLocation, lineRange };
            FileLocation = filePassedByCompiler;
            StartingLineNumber = linePassedByCompiler;
        }

        /// <summary>
        /// Constructor for documentation group (containing multiple snippets)
        /// </summary>
        /// <param name="details"></param>
        /// <param name="guard"></param>
        /// <param name="filePassedByCompiler"></param>
        /// <param name="linePassedByCompiler"></param>
        public DemonstratedByAttribute(string[] details, 
                                       ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                       [CallerFilePath] string filePassedByCompiler = Default.CompilerServiceFile, 
                                       [CallerLineNumber] int linePassedByCompiler = Default.CompilerServiceLineNumber)
        {
            Generated = true;
            DocumentationDetails = details;
            FileLocation = filePassedByCompiler;
            StartingLineNumber = linePassedByCompiler;
        }
    }
}