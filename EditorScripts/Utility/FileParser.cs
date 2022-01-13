using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class FileParser
    {
        private static readonly char[] WhiteSpaceCharacters = {' ', '\t'};
        public static LineNumberRange GetLineNumberRangeForAttribute(int startingLineNumber, string filePath)
        {
            return GetRangeBetweenCharacters(filePath, startingLineNumber, CharacterPair.SquareBrackets, true);
        }

        public static LineNumberRange GetLineNumberRangeForMember(MemberInfo memberInfo,
                                                                  string fileLocation,
                                                                  RelevantArea relevantArea,
                                                                  int beginSearchLineNumber)
        {
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
        

        public static int FindDeclarationStartLineForMember(MemberInfo memberInfo, string fileLocation, int beginSearchLineNumber)
        {
            string[] lines = File.ReadAllLines(fileLocation);
            int initialIndex = beginSearchLineNumber - 1;
            for (var index = initialIndex; index < lines.Length; index++)
            {
                string[] tokens = lines[index].Split(WhiteSpaceCharacters);
            }

            return 0;
        }

        public static LineNumberRange GetRangeBetweenCharacters(string fileLocation,
                                                                int beginSearchLineNumber,
                                                                CharacterPair pair,
                                                                bool includeCharacterLines)
        {
            char open = default, close = default;
            switch (pair)
            {
                case CharacterPair.Parenthesis:
                    open = '(';
                    close = ')';
                    break;
                case CharacterPair.CurlyBrackets:
                    open = '{';
                    close = '}';
                    break;
                case CharacterPair.SquareBrackets:
                    open = '[';
                    close = ']';
                    break;
            }
            
            return GetRangeBetweenCharacters(File.ReadAllLines(fileLocation), beginSearchLineNumber, open, close, includeCharacterLines);

        }
        
        private enum SearchState
        {
            SearchingForOpenCharacter,
            SearchingForCloseCharacter,
        }
        public static LineNumberRange GetRangeBetweenCharacters(string[] lines, 
                                                                int beginSearchLineNumber, 
                                                                char openCharacter, 
                                                                char closeCharacter, 
                                                                bool includeCharacterLines)
        {
            int initialIndex = beginSearchLineNumber - 1;
            int rangeStart = default, openCharacterCount = default;
            bool insideQuote = default;
            bool IsOpenChar(char character) => character == openCharacter;
            bool IsCloseChar(char character) => character == closeCharacter;
            bool IsQuote(char character) => character == '"';
            bool IsInsideQuote(char character)
            {
                insideQuote = insideQuote ? !IsQuote(character) : IsQuote(character);
                return insideQuote;
            }
            SearchState state = SearchState.SearchingForOpenCharacter;
            for (var index = initialIndex; index < lines.Length; index++)
            {
                foreach (char character in lines[index])
                {
                    if (IsInsideQuote(character))
                    {
                        continue;
                    }
                    switch (state)
                    {
                        case SearchState.SearchingForOpenCharacter:
                            if (IsOpenChar(character))
                            {
                                state = SearchState.SearchingForCloseCharacter;
                                openCharacterCount = 1;
                                rangeStart = index + (includeCharacterLines ? 0 : 1);
                            }
                            break;
                        case SearchState.SearchingForCloseCharacter:
                            openCharacterCount += (IsOpenChar(character) ? 1 : 0) - (IsCloseChar(character) ? 1 : 0);
                            if (openCharacterCount == 0)
                            {
                                int rangeEnd = index - (includeCharacterLines ? 0 : 1);
                                return new LineNumberRange(rangeStart + 1, rangeEnd + 1);
                            }
                            break;
                    }
                }
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