using System;
using System.Runtime.CompilerServices;

namespace pbuddy.TestsAsDocumentationUtility.RuntimeScripts
{
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class IsDemonstratedByTestsAttribute : Attribute
    {
        private const string FilledInByCompiler = "[Filled in by Compiler]";
        private const int InvalidLineNumber = -1;
        public int StartingLineNumber { get; }
        public string FileLocation { get; }
        public IsDemonstratedByTestsAttribute([CallerFilePath] string filePassedByCompiler = FilledInByCompiler, 
                                              [CallerLineNumber] int linePassedByCompiler = InvalidLineNumber)
        {
            FileLocation = filePassedByCompiler;
            StartingLineNumber = linePassedByCompiler;
        }
    }
}