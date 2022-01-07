using System;
using System.Runtime.CompilerServices;

namespace pbuddy.TestsAsDocumentationUtility.RuntimeScripts
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DocumentedByAttribute : Attribute
    {
        public DocumentedByAttribute(ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard,
                                     [CallerFilePath] string file = CompilerServicesDefaults.File,
                                     [CallerLineNumber] int line = CompilerServicesDefaults.LineNumber)
        {
        }
        
        public DocumentedByAttribute(string fileLocation,
                            string memberName,
                            ArgumentGuard guard = ArgumentGuard.GeneratedArgumentsGuard,
                            [CallerFilePath] string file = CompilerServicesDefaults.File,
                            [CallerLineNumber] int line = CompilerServicesDefaults.LineNumber)
        {
            
        }
    }
}