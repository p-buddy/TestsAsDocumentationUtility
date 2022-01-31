using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using pbuddy.TestsAsDocumentationUtility.EditorScripts.Declarations;

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

        public static int FindDeclarationStartLine(MemberInfo memberInfo, string fileLocation, int beginSearchLineNumber)
        {
            const string matchAnyWhiteSpaceCharacter = "\\s";
            string[] lines = File.ReadAllLines(fileLocation);
            int initialIndex = LineNumberToIndex(beginSearchLineNumber);
            RegexOptions options = RegexOptions.Multiline;
            string pattern = CodeMatcher.GetDeclarationRegex(memberInfo);
            for (var index = initialIndex; index < lines.Length; index++)
            {
                var tokens = lines[index].Split();
                if (tokens.Length == 0)
                {
                    continue;
                }
                
                if (tokens[0].StartsWith("//"))
                {
                    continue;
                }

                if (tokens[0].StartsWith("["))
                {
                    (char open, char close) query = GetCharacters(CharacterPair.SquareBrackets);
                    int lineNumber = IndexToLineNumber(index);
                    LineNumberRange range = GetRangeBetweenCharacters(lines, lineNumber, query, true);
                    index = LineNumberToIndex(range.End) - 1;
                    continue;
                }

                if (Regex.Matches(lines[index], pattern, options).Count > 0)
                {
                    return IndexToLineNumber(index);
                }
            }

            return 0;
        }
        
        public static LineNumberRange GetRangeBetweenCharacters(string fileLocation,
                                                                int beginSearchLineNumber,
                                                                CharacterPair pair,
                                                                bool includeCharacterLines)
        {
            (char open, char close) c = GetCharacters(pair);
            return GetRangeBetweenCharacters(File.ReadAllLines(fileLocation), beginSearchLineNumber, c.open, c.close, includeCharacterLines);

        }

        public static (char open, char close) GetCharacters(CharacterPair pair)
        {
            switch (pair)
            {
                case CharacterPair.Parenthesis:
                    return ('(', ')');
                case CharacterPair.CurlyBrackets:
                    return ('{', '}');
                case CharacterPair.SquareBrackets:
                    return ('[', ']');
                case CharacterPair.DoubleQuote:
                    return ('\"', '\"');
            }

            return default;
        }

        private enum SearchState
        {
            SearchingForOpenCharacter,
            SearchingForCloseCharacter,
        }

        public static LineNumberRange GetRangeBetweenCharacters(string[] lines,
                                                                int beginSearchLineNumber,
                                                                (char open, char close) query,
                                                                bool includeCharacterLines)
        {
            return GetRangeBetweenCharacters(lines, beginSearchLineNumber, query.open, query.close, includeCharacterLines);
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
            bool insideMultiLineComment = default;
            bool IsOpenChar(char character) => character == openCharacter;
            bool IsCloseChar(char character) => character == closeCharacter;
            bool IsQuote(char character) => character == '"';
            bool IsSlash(char character) => character == '/';
            bool IsAstrix(char character) => character == '*';
            
            bool IsInsideQuote(char character)
            {
                insideQuote = insideQuote ? !IsQuote(character) : IsQuote(character);
                return insideQuote;
            }
            
            bool IsInsideMultiLineComment(char character, char previousCharacter)
            {
                insideMultiLineComment = insideMultiLineComment
                    ? !(IsAstrix(previousCharacter) && IsSlash(character))
                    : IsAstrix(character) && IsSlash(previousCharacter);
                return insideMultiLineComment;
            }

            bool IsSingleLineComment(char character, char previousCharacter) =>
                IsSlash(character) && IsSlash(previousCharacter);
            
            
            SearchState state = SearchState.SearchingForOpenCharacter;
            for (var index = initialIndex; index < lines.Length; index++)
            {
                char previousCharacter = default;
                foreach (char character in lines[index])
                {
                    if (IsInsideQuote(character))
                    {
                        goto SetPreviousCharacter;
                    }

                    if (IsInsideMultiLineComment(character, previousCharacter))
                    {
                        goto SetPreviousCharacter;
                    }
                    
                    if (IsSingleLineComment(character, previousCharacter))
                    {
                        break;
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
                    
                    SetPreviousCharacter:
                        previousCharacter = character;
                }
            }

            throw new Exception();
        }

        private static LineNumberRange GetMethodBodyRange(int attributeLineNumber, string[] lines)
        {
            (char open, char close) c = GetCharacters(CharacterPair.CurlyBrackets);
            return GetRangeBetweenCharacters(lines, attributeLineNumber, c.open, c.close, false);
        }
        
        private static LineNumberRange GetBodyAndDeclarationRange(int attributeLineNumber, string[] lines)
        {
            LineNumberRange bodyRange = GetMethodBodyRange(attributeLineNumber, lines);
            return new LineNumberRange(bodyRange.Start /*TODO*/, bodyRange.End + 1);
        }

        private static int LineNumberToIndex(int lineNumber) => lineNumber - 1;
        private static int IndexToLineNumber(int lineNumber) => lineNumber + 1;

    }
}