using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts;
using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;

namespace pbuddy.TestsAsDocumentationUtility.EditModeTests
{
    public class FileReaderTests
    {
        [Test]
        public void GetRangeBetweenCurlyBracketsExclusive()
        {
            int expectedStart = GetThisLineNumber();
            string thisFileName = GetThisFileName();
            const int lookBehind = 2;
            LineNumberRange actualRange = FileParser.GetRangeBetweenCharacters(thisFileName,
                expectedStart - lookBehind,
                CharacterPair.CurlyBrackets,
                false);
            int expectedEnd = GetThisLineNumber() + 2;
            LineNumberRange expectedRange = new LineNumberRange(expectedStart, expectedEnd);
            Assert.AreEqual(expectedRange, actualRange);
        }
        
        [Test]
        public void GetRangeBetweenCurlyBracketsInclusive()
        {
            int expectedStart = GetThisLineNumber() - 1;
            string thisFileName = GetThisFileName();
            const int lookBehind = 2;
            LineNumberRange actualRange = FileParser.GetRangeBetweenCharacters(thisFileName,
                expectedStart - lookBehind,
                CharacterPair.CurlyBrackets,
                true);
            int expectedEnd = GetThisLineNumber() + 3;
            LineNumberRange expectedRange = new LineNumberRange(expectedStart, expectedEnd);
            Assert.AreEqual(expectedRange, actualRange);
        }
        
        [Test]
        [GetLineNumberAndFile(Dummy.Argument, 
                              Dummy.Argument, 
                              Dummy.Argument, 
                              4)]
        [GetLineNumberAndFile("test[][][]", 1)]
        [GetLineNumberAndFile("test[[[[[[",
                              2)]
        [GetLineNumberAndFile("test ]" +
                              "",
                              3)]
        [GetLineNumberAndFile(new object[0],
                              3)
        ]
        [GetLineNumberAndFile("test ]" +
                              "",/*
                              
                              ] [] // ]]]]]
                              */
                              6)]
        [GetLineNumberAndFile("test ]" +
                              "",
                              4/*"]"*/)
        ]
        [GetLineNumberAndFile("test ]", "]", "]",
                              3/*"]"*/)
        ]
        [GetLineNumberAndFile("test ]", 
                              "]", 
                              "]",
                              5/*"]"*/)
        ]
        [GetLineNumberAndFile("test ]" +
                              "", // ]
                              
                              // ]
                              5)]
        [GetLineNumberAndFile("test ]" +
                              "",
                              /*
                               
                                ]
                               */
                              8/**/)
        ]
        public void GetRangeBetweenAttributeDeclaration()
        {
            Type type = GetType();
            MethodInfo thisMethod = type.GetMethod(nameof(GetRangeBetweenAttributeDeclaration));
            Assert.IsNotNull(thisMethod);
            GetLineNumberAndFileAttribute[] attributes = thisMethod.GetCustomAttributes<GetLineNumberAndFileAttribute>().ToArray();
            foreach (GetLineNumberAndFileAttribute attribute in attributes)
            {
                LineNumberRange actualRange = FileParser.GetRangeBetweenCharacters(attribute.FileName,
                                                               attribute.LineNumberStart,
                                                               CharacterPair.SquareBrackets,
                                                               true);
                Assert.AreEqual(attribute.ExpectedRange, actualRange);
            }
        }

        private static int GetThisLineNumber([CallerLineNumber] int line = default) => line;
        private static string GetThisFileName([CallerFilePath] string file = "") => file;

        private enum Dummy
        {
            Argument
        }
        
        
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        private class GetLineNumberAndFileAttribute : Attribute
        {
            public int LineNumberStart { get; }
            public string FileName { get; }
            public LineNumberRange ExpectedRange { get; }
            
            private GetLineNumberAndFileAttribute(int lineCount, int lineNumberFilledInByCompiler, string fileNameFilledInByCompiler)
            {
                LineNumberStart = lineNumberFilledInByCompiler;
                FileName = fileNameFilledInByCompiler;
                ExpectedRange = new LineNumberRange(LineNumberStart, LineNumberStart + lineCount - 1);
            }
            
            public GetLineNumberAndFileAttribute(Dummy a, 
                                                 Dummy b, 
                                                 Dummy c,
                                                 int lineCount,
                                                 [CallerLineNumber] int lineNumberFilledInByCompiler = default,
                                                 [CallerFilePath] string fileNameFilledInByCompiler = "") 
                : this(lineCount, lineNumberFilledInByCompiler, fileNameFilledInByCompiler)
            {
            }
            
            public GetLineNumberAndFileAttribute(string text,
                                                 int lineCount,
                                                 [CallerLineNumber] int lineNumberFilledInByCompiler = default,
                                                 [CallerFilePath] string fileNameFilledInByCompiler = "")
                : this(lineCount, lineNumberFilledInByCompiler, fileNameFilledInByCompiler)
            {
            }
            
            public GetLineNumberAndFileAttribute(string text1,
                                                 string text2,
                                                 string text3,
                                                 int lineCount,
                                                 [CallerLineNumber] int lineNumberFilledInByCompiler = default,
                                                 [CallerFilePath] string fileNameFilledInByCompiler = "")
                : this(lineCount, lineNumberFilledInByCompiler, fileNameFilledInByCompiler)
            {
            }
            
            public GetLineNumberAndFileAttribute(object[] array,
                                                 int lineCount,
                                                 [CallerLineNumber] int lineNumberFilledInByCompiler = default,
                                                 [CallerFilePath] string fileNameFilledInByCompiler = "")
                : this(lineCount, lineNumberFilledInByCompiler, fileNameFilledInByCompiler)
            {
            }
        }
    }
}