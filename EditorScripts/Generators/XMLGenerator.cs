using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class XMLGenerator
    {
        private const string Example = "example";
        private const string Paragraph = "para";
        private const string Code = "code lang=\"C#\"";
        private const string Bold = "b";
        private const string Italic = "i";
        private const string Indentation = "\t\t";
        
        private enum TagID
        {
            GroupTitle,
            GroupDescription,
            SnippetTitle,
            SnippetDescription,
            CodeSnippet, 
            TestAsDocumentation,
            None
        }
        
        public static string GenerateXML(this DocumentationCollection docCollection)
        {
            DocumentationGroup[] groups = docCollection.GetGroups();
            string[] xmlPerGroup = new string[groups.Length];
            
            int callCount = 0;
            string ToTitle(string text)
            {
                string title = $"Example {callCount + 1}" + (text != null ? $": {text}" : String.Empty);
                callCount++;
                return title;
            }
            
            for (int i = 0; i < groups.Length; i++)
            {
                xmlPerGroup[i] = groups[i].GenerateXML(ToTitle);
            }

            return String.Join(Environment.NewLine, xmlPerGroup);
        }
        
        private static string GenerateXML(this in DocumentationGroup docGroup, Func<string, string> toExampleTitle)
        {
            List<DocumentationSnippet> docs = docGroup.GetSnippets();
            StringBuilder xml = new StringBuilder();

            void Add(params string[] text)
            {
                if (text == null || text.Length == 0)
                {
                    return;
                }
                xml.Append(String.Join(Environment.NewLine, text));
                xml.Append(Environment.NewLine);
            }

            (string title, string description) groupText = docs.GetGroupTitleAndDescription(docGroup.Group);

            if (docGroup.Group == Grouping.None)
            {
                foreach (DocumentationSnippet doc in docs)
                {
                    Add(OpenXml(Example, TagID.TestAsDocumentation));
                    Add(ExampleTitleToXml(toExampleTitle.Invoke(doc.Title)));
                    Add(ExampleDescriptionToXml(doc.Description));
                    Add(CodeToXml(doc.GetContents()));
                    Add(CloseXml(Example));
                }

                return xml.ToString();
            }
            
            Add(OpenXml(Example, TagID.TestAsDocumentation));
            Add(ExampleTitleToXml(toExampleTitle.Invoke(groupText.title)));
            Add(ExampleDescriptionToXml(groupText.description));
            foreach (DocumentationSnippet doc in docs)
            {
                Add(SnippetTitleToXml(doc.Title));
                Add(SnippetDescriptionToXml(doc.Description));
                Add(CodeToXml(doc.GetContents()));
            }
            Add(CloseXml(Example));

            return xml.ToString();
        }

        private static string TagIDText(TagID id) => Enum.GetName(typeof(TagID), id);
        private static string IdentifierLabel(TagID id) => $"identifier=\"{TagIDText(id)}\"";
        private static string OpenXml(string tag, TagID id = TagID.None) =>
            id == TagID.None
                ? $"<{tag}>"
                : $"<{tag} {IdentifierLabel(id)}>";
        private static string CloseXml(string tag) => $"</{tag}>";
        private static string[] WrapInXml(string tag, TagID id, params string[] text)
        {
            if (text.Length == 0 || String.IsNullOrEmpty(text[0]))
            {
                return null;
            }
            string[] wrapped = new string[text.Length + 2];
            wrapped[0] = OpenXml(tag, id);
            string closing = tag.Split(' ')[0];
            wrapped[text.Length + 1] = CloseXml(closing);
            Array.Copy(text, 0, wrapped, 1, text.Length);
            return wrapped;
        }
        
        private static string[] WrapInXml(string tag, params string[] text)
        {
            return WrapInXml(tag, TagID.None, text);
        }
        private static string[] ExampleTitleToXml(string title) => WrapInXml(Paragraph, TagID.GroupTitle, WrapInXml(Bold, title)); 
        private static string[] ExampleDescriptionToXml(string description) => WrapInXml(Paragraph, TagID.GroupDescription, description);
        private static string[] SnippetTitleToXml(string title) => WrapInXml(Paragraph, TagID.SnippetTitle, WrapInXml(Italic, title));
        private static string[] SnippetDescriptionToXml(string description) => WrapInXml(Paragraph, TagID.SnippetDescription, description);
        private static string[] CodeToXml(string contents) => WrapInXml(Code, TagID.CodeSnippet, $"<![CDATA[{Environment.NewLine}{contents}]]>");
    }
}