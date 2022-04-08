using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class GeneratorUtility
    {
        private const string ErrorContext = "[" + nameof(GeneratorUtility) + " ERROR]: ";
        private static readonly string NoneGroup = Enum.GetName(typeof(Grouping), Grouping.None);
        private const string StartOfLine = "^";
        private const string ZeroOrMoreTimes = "*";
        private const string MatchZeroOrMoreWhitespace = @"\s" + ZeroOrMoreTimes;
        private const string MatchOpeningWhiteSpace = StartOfLine + MatchZeroOrMoreWhitespace;
        private const string MatchZeroOrMoreDigits = @"\d" + ZeroOrMoreTimes;
        private const string GroupDigits = "(" + MatchZeroOrMoreDigits + ")";
        private const string MatchOpenParenthesis = @"\(";
        private const string MatchCloseParenthesis = @"\)";
        private const string MatchOpenBracket = @"\[";
        private const string MatchCloseBracket = @"\]";
        private const string HighlightCallPattern = MatchOpeningWhiteSpace +
                                                  "HighlightNext" +
                                                  MatchOpenParenthesis +
                                                  GroupDigits +
                                                  MatchCloseParenthesis;

        private static readonly string HighlightAttributePattern = MatchOpeningWhiteSpace +
                                                                 MatchOpenBracket +
                                                                 nameof(HighlightNextAttribute)
                                                                     .RemoveSubString("Attribute") +
                                                                 MatchOpenParenthesis +
                                                                 ZeroOrMoreTimes +
                                                                 GroupDigits +
                                                                 MatchCloseParenthesis +
                                                                 ZeroOrMoreTimes +
                                                                 MatchCloseBracket;

        private static readonly Regex HighlightCallRegex = new Regex(HighlightCallPattern);
        private static readonly Regex HighlightAttributeRegex = new Regex(HighlightAttributePattern);
        

        public static (string, string) GetGroupTitleAndDescription(this List<DocumentationSnippet> docs, Grouping grouping)
        {
            List<DocumentationSnippet> withTitle = docs.Where(doc => doc.GroupInfo.GroupTitle != null).ToList();
            
            if (grouping == Grouping.None && withTitle.Count > 0)
            {
                string subject = withTitle[0].MemberBeingDocumented.GetReadableName();
                IEnumerable<string> problems = withTitle.Select(doc => doc.MemberDoingTheDocumenting.GetReadableName());
                throw new NotSupportedException(ErrorContext +
                                                $"Documenting members in the ${NoneGroup} should NOT provide group titles. " +
                                                $"The following members (documenting {subject}) do:{Environment.NewLine}" +
                                                $"{String.Join(Environment.NewLine, problems)}");
            }
            
            if (withTitle.Count > 1)
            {
                string subject = withTitle[0].MemberBeingDocumented.GetReadableName();
                IEnumerable<string> problems = withTitle.Select(doc => doc.MemberDoingTheDocumenting.GetReadableName());
                throw new NotSupportedException(ErrorContext +
                                                "Only ONE documenting member in a group should provide a group title. " +
                                                $"All of the following members documenting {subject} provided a group title:{Environment.NewLine}" + 
                                                $"{String.Join(Environment.NewLine, problems)}");
            }
            
            List<DocumentationSnippet> withDescription = docs.Where(doc => doc.GroupInfo.GroupDescription != null).ToList();
            
            if (grouping == Grouping.None && withDescription.Count > 0)
            {
                string subject = withDescription[0].MemberBeingDocumented.GetReadableName();
                IEnumerable<string> problems = withDescription.Select(doc => doc.MemberDoingTheDocumenting.GetReadableName());
                throw new NotSupportedException(ErrorContext +
                                                $"Documenting members in the ${NoneGroup} should NOT provide group descriptions. " +
                                                $"The following members (documenting {subject}) do:{Environment.NewLine}" +
                                                $"{String.Join(Environment.NewLine, problems)}");            }
            
            if (withDescription.Count > 1)
            {
                string subject = withDescription[0].MemberBeingDocumented.GetReadableName();
                IEnumerable<string> problems = withDescription.Select(doc => doc.MemberDoingTheDocumenting.GetReadableName());
                throw new NotSupportedException(ErrorContext +
                                                "Only ONE documenting member in a group should provide a group descriptions. " +
                                                $"All of the following members documenting {subject} provided a group title:{Environment.NewLine}" + 
                                                $"{String.Join(Environment.NewLine, problems)}");            }

            string title = withTitle.Count == 1 ? withTitle[0].GroupInfo.GroupTitle : null;
            string description = withDescription.Count == 1 ? withDescription[0].GroupInfo.GroupDescription : null;
            return (title, description);
        }

        public static string ExtractContentsWithoutHighlights(in this DocumentationSnippet snippet)
        {
            throw new NotImplementedException();
        }
        
        public static void ExtractContentsAndRelativeHighlightRanges(in this DocumentationSnippet snippet,
                                                                     out string codeSnippet,
                                                                     out LineNumberRange[] highlightRanges)
        {
            throw new NotImplementedException();
        }
        
        public static void ExtractGlobalSnippetAndHighlightRanges(in this DocumentationSnippet snippet, 
                                                                  out LineNumberRange[] snippetRanges, 
                                                                  out LineNumberRange[] highlightRanges)
        {
            throw new NotImplementedException();
        }

        public static void ExtractHighlightRanges(string codeSnippet,
                                                  out string editedSnippet,
                                                  out LineNumberRange[] highlightRanges)
        {
            static bool TryGetLinesToHighlight(Match match, out int lineCount)
            {
                if (!match.Success)
                {
                    lineCount = default;
                    return false;
                }

                if (match.Groups.Count == 1)
                {
                    lineCount = 1;
                    return true;
                }

                string parameter = match.Groups[1].Captures[0].Value;
                if (Int32.TryParse(parameter, out lineCount))
                {
                    return true;
                }

                throw new Exception(ErrorContext + "Couldn't extract line count");
            }
            
            string[] originalLines = codeSnippet.Split(new [] { Environment.NewLine }, StringSplitOptions.None);
            List<string> editedLines = new List<string>();
            List<LineNumberRange> ranges = new List<LineNumberRange>();

            
            for (int i = 0; i < originalLines.Length; i++)
            {
                string line = originalLines[i];
                Match functionMatch = HighlightCallRegex.Match(line);
                if (TryGetLinesToHighlight(functionMatch, out int count))
                {
                    int min = Math.Min(i + 1, originalLines.Length);
                    // int max = Math.Min(i + 1)
                    // skip this line, but add count, but make sure to avoid index out of range
                    i += count - 1;
                    continue;
                }
                
                Match attributeMatch = HighlightAttributeRegex.Match(line);
                if (TryGetLinesToHighlight(attributeMatch, out count))
                {
                    // skip this line, but add count, but make sure to avoid index out of range
                    i += count - 1;
                    continue;
                }
                
                editedLines.Add(line);
            }
            
            editedSnippet = String.Join(Environment.NewLine, editedLines);
            highlightRanges = default;
        }
    }
}