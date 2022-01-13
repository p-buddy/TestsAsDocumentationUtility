using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace pbuddy.TestsAsDocumentationUtility.RuntimeScripts
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DemonstratedByAttribute : Attribute
    {
        public bool Valid { get; }
        public int StartingLineNumber { get; }
        public string FileLocation { get; }
        public string DocumentingFileLocation { get; }
        public string DocumentingName { get; }
        
        public DemonstratedByAttribute(ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                       [CallerFilePath] string filePassedByCompiler = Default.CompilerServiceFile, 
                                       [CallerLineNumber] int linePassedByCompiler = Default.CompilerServiceLineNumber)
        {
            Valid = false;
            FileLocation = filePassedByCompiler;
            StartingLineNumber = linePassedByCompiler;
        }
        
        public DemonstratedByAttribute(string fileLocation, 
                                       string name, 
                                       ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                       [CallerFilePath] string filePassedByCompiler = Default.CompilerServiceFile, 
                                       [CallerLineNumber] int linePassedByCompiler = Default.CompilerServiceLineNumber)
        {
            Valid = true;
            DocumentingFileLocation = fileLocation;
            DocumentingName = name;
            FileLocation = filePassedByCompiler;
            StartingLineNumber = linePassedByCompiler;
        }
    }
}