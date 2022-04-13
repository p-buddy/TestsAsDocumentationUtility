using System;
using System.Text.RegularExpressions;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class StringHelper
    {
        private const string Space = " ";
        private const string CamelCasePattern = "(?<=[A-Z])(?=[A-Z][a-z]) | (?<=[^A-Z])(?=[A-Z]) | (?<=[A-Za-z])(?=[^A-Za-z])";
        private static readonly Regex CamelCaseRegex = new Regex(CamelCasePattern, RegexOptions.IgnorePatternWhitespace);
        public static string DelimitCamelCase(this string camelCase, string delimiter = " ") =>
            CamelCaseRegex.Replace(camelCase, delimiter);
        public static string ErrorContext<T>(this string functionName, bool endingSpace = false) =>
            $"[{nameof(T)} Error in ${functionName}()]:{(endingSpace ? Space : String.Empty)}";
        public static string ErrorContext<T>(this T self, string functionName, bool endingSpace = false) =>
            $"[{nameof(T)} Error in ${functionName}()]:{(endingSpace ? Space : String.Empty)}";
        public static string GetLeadingWhitespace(this string str)
        {
            return str.Replace(str.TrimStart(), "");
        }
        
        public static string RemoveSubString(this string fullString, string substring)
        {
            int indexOf = fullString.IndexOf(substring, StringComparison.Ordinal);
            if (indexOf < 0 || indexOf >= fullString.Length)
            {
                return fullString;
            }
            
            int length = substring.Length;
            return fullString.Remove(indexOf, length);
        }
    }
}