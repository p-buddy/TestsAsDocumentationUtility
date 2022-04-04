using System;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class StringHelper
    {
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