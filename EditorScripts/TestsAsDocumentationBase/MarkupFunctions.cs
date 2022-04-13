using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public abstract partial class TestsAsDocumentationBase
    {
        private const string FilledInByCompiler = "[Filled in by Compiler]";
        private const int InvalidLineNumber = -1;

        private static readonly List<IMarkup> CurrentMarkups = new List<IMarkup>();
        
        protected void HighlightNext(uint numberOfLines = 1,
                                     string detail = null,
                                     AppliesTo? appliesTo = null,
                                     [CallerFilePath] string file = FilledInByCompiler,
                                     [CallerLineNumber] int line = InvalidLineNumber)
        {
            CurrentMarkups.Add(new Highlight(file, line, detail, numberOfLines, appliesTo));
        }
        
        protected void RemoveNext(uint numberOfLines,
                                  AppliesTo? appliesTo = null,
                                  [CallerFilePath] string file = FilledInByCompiler,
                                  [CallerLineNumber] int line = InvalidLineNumber)
        {
            CurrentMarkups.Add(new Removal(file, line, numberOfLines, appliesTo));
        }

        protected void AddHere(string contents, 
                               AppliesTo? appliesTo = null, 
                               [CallerFilePath] string file = FilledInByCompiler, 
                               [CallerLineNumber] int line = InvalidLineNumber)
        {
            CurrentMarkups.Add(new Addition(file, line, contents, appliesTo));
        }
    }
}