using System;
using System.Runtime.CompilerServices;

namespace pbuddy.TestsAsDocumentationUtility.RuntimeScripts
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DemonstratedByAttribute : Attribute
    {
        public bool Valid { get; }
        public int StartingLineNumber { get; }
        public string FileLocation { get; }
        public string DocumentingName { get; }

        
        public DemonstratedByAttribute(ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                       [CallerFilePath] string file = CompilerServicesDefaults.File, 
                                       [CallerLineNumber] int line = CompilerServicesDefaults.LineNumber)
        {
            Valid = false;
            FileLocation = file;
            StartingLineNumber = line;
        }
        
        public DemonstratedByAttribute(string fileLocation, 
                                       string name, 
                                       ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard, 
                                       [CallerFilePath] string file = CompilerServicesDefaults.File, 
                                       [CallerLineNumber] int line = CompilerServicesDefaults.LineNumber)
        {
            Valid = true;
            FileLocation = file;
            StartingLineNumber = line;
            DocumentingName = name;
        }
    }
}