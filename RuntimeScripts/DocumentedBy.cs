using System;
using System.Runtime.CompilerServices;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts.pbuddy.TestsAsDocumentationUtility.RuntimeScripts
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class DocumentedBy : Attribute
    {
        private const String DummyFileName = "<[Ignore the rest]>";
        private const int DummyLineNumber = -1;
        

        public DocumentedBy([CallerFilePath] string file = DummyFileName, [CallerLineNumber] int line = DummyLineNumber)
        {
        }
        
        public DocumentedBy(string fileLocation,
                            string memberName,
                            [CallerFilePath] string file = DummyFileName,
                            [CallerLineNumber] int line = DummyLineNumber)
        {
            
        }
    }
}