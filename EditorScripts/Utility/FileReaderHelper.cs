using System;
using System.IO;
using System.Linq;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class FileReaderHelper
    {
        public static LineNumberRange GetLineNumberRangeForAttribute(int startingLineNumber, 
                                                                     string filePath)
        {
            // TODO
            return default;
        }
        
        public static LineNumberRange GetLineNumberRange(int startingLineNumber, 
                                                         string filePath, 
                                                         RelevantArea relevantArea)
        {
            string[] lines = File.ReadAllLines(filePath);
            switch (relevantArea)
            {
                case RelevantArea.BodyOnly:
                    return GetMethodBodyRange(startingLineNumber, lines);
                case RelevantArea.DeclarationAndBody:
                    return GetBodyAndDeclarationRange(startingLineNumber, lines);
            }

            throw new Exception();
        }
        
        private static LineNumberRange GetMethodBodyRange(int attributeLineNumber, string[] lines)
        {
            const char openCurlyBrace = '{';
            const char closeCurlyBrace = '}';
            int lineIndex = attributeLineNumber - 1;
            int openCurlyBraceCount = 0;
            
            int GetCharacterCount(char character) => lines[lineIndex].Count(c => c == character);
            int GetOpenCount() => GetCharacterCount(openCurlyBrace) - GetCharacterCount(closeCurlyBrace);
            void Step()
            {
                openCurlyBraceCount += GetOpenCount();
                lineIndex++;
            }
            
            // Step to method's opening bracket
            while (openCurlyBraceCount == 0)
            {
                Step();
            }
            int lineStart = lineIndex + 1;
            
            // Step until closing bracket
            while (openCurlyBraceCount > 0)
            {
                Step();
            }

            var lineEnd = lineIndex - 1;
            return new LineNumberRange(lineStart, lineEnd);
        }
        
        private static LineNumberRange GetBodyAndDeclarationRange(int attributeLineNumber, string[] lines)
        {
            LineNumberRange bodyRange = GetMethodBodyRange(attributeLineNumber, lines);
            return new LineNumberRange(bodyRange.Start /*TODO*/, bodyRange.End + 1);
        }
    }
}