using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class FileParser
    {
        public static LineNumberRange GetLineNumberRangeForAttribute(int startingLineNumber, string filePath)
        {
            return GetRangeBetweenCharacters(filePath, startingLineNumber, CharacterPair.SquareBrackets, true);
        }

        public static LineNumberRange GetLineNumberRangeForMember(MemberInfo memberInfo,
                                                                  string fileLocation,
                                                                  RelevantArea relevantArea,
                                                                  int beginSearchLineNumber)
        {
            LineNumberRange attributesRange = GetAttributesRange<DemonstratesAttribute>(memberInfo);
            int declarationStartLine = GetDeclarationStartLine(memberInfo, fileLocation, beginSearchLineNumber);
            string[] lines = File.ReadAllLines(fileLocation);
            
            switch (relevantArea)
            {
                case RelevantArea.BodyOnly:
                    return GetBodyRange(declarationStartLine, lines, false);
                case RelevantArea.DeclarationAndBody:
                {
                    LineNumberRange fullBodyRange = GetBodyRange(declarationStartLine, lines, true);
                    return new LineNumberRange(declarationStartLine, fullBodyRange.End);
                }
                case RelevantArea.DeclarationAndBodyAndBelowAttributes:
                {                  
                    LineNumberRange fullBodyRange = GetBodyRange(declarationStartLine, lines, true);
                    return new LineNumberRange(attributesRange.End + 1, fullBodyRange.End); 
                }
            }
            throw new Exception("");
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
        
        public static int GetDeclarationStartLine(MemberInfo memberInfo, string fileLocation, int startSearchLineNumber)
        {
            string regex = CodeMatcher.GetDeclarationRegex(memberInfo);
            string contents = File.ReadAllText(fileLocation).Substring(startSearchLineNumber);
            var match = Regex.Match(contents, regex);
            string sub = contents.Substring(0, match.Index);
            return sub.Split(new [] { Environment.NewLine }, StringSplitOptions.None).Length;
        }

        private static LineNumberRange GetBodyRange(int attributeLineNumber, string[] lines, bool includeCharacterLines)
        {
            (char open, char close) c = GetCharacters(CharacterPair.CurlyBrackets);
            return GetRangeBetweenCharacters(lines, attributeLineNumber, c.open, c.close, includeCharacterLines);
        }

        private static LineNumberRange GetAttributesRange<TAttributeType>(MemberInfo memberInfo) where TAttributeType: Attribute, ILineNumberRangeProvider
        {
            int min = int.MaxValue, max = int.MinValue;
            foreach (var attribute in memberInfo.GetCustomAttributes<TAttributeType>())
            {
                LineNumberRange range = attribute.LineNumberRange;
                min = range.Start < min ? range.Start : min;
                max = range.End > max ? range.End : max;
            }

            return new LineNumberRange(min, max);
        }
    }
}