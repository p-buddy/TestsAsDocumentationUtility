using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class XMLGenerator
    {
        private const string ErrorContext = "[" + nameof(XMLGenerator) + " ERROR]: ";
        private static readonly string NoneGroup = Enum.GetName(typeof(Grouping), Grouping.None);
        
        public static string GenerateXML(this in DocumentationGroup docGroup, Func<string, string> toExampleTitle)
        {
            List<Documentation> docs = docGroup.GetDocuments();
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

            (string title, string description) groupText = GetGroupTitleAndDescription(docGroup.Group, docs);

            if (docGroup.Group == Grouping.None)
            {
                foreach (Documentation doc in docs)
                {
                    Add(OpenXml(Example));
                    Add(ExampleTitleToXml(toExampleTitle.Invoke(doc.Title)));
                    Add(ExampleDescriptionToXml(doc.Description));
                    Add(CodeToXml(doc.GetContents()));
                    Add(CloseXml(Example));
                }

                return xml.ToString();
            }
            
            Add(OpenXml(Example));
            Add(ExampleTitleToXml(toExampleTitle.Invoke(groupText.title)));
            Add(ExampleDescriptionToXml(groupText.description));
            foreach (Documentation doc in docs)
            {
                Add(SnippetTitleToXml(doc.Title));
                Add(SnippetDescriptionToXml(doc.Description));
                Add(CodeToXml(doc.GetContents()));
            }
            Add(CloseXml(Example));

            return xml.ToString();
        }

        private const string Example = "example";
        private const string Paragraph = "para";
        private const string Code = "code lang=\"C#\"";
        private const string Bold = "b";
        private const string Italic = "i";
        
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

        private static string TagIDText(TagID id) => Enum.GetName(typeof(TagID), id);
        private static string OpenXml(string tag) => $"<{tag}>";
        private static string CloseXml(string tag) => $"</{tag}>";
        private static string[] WrapInXml(string tag, TagID id, params string[] text)
        {
            if (text.Length == 0 || text[0] == null)
            {
                return null;
            }
            var wrapped = new string[text.Length + 2];
            wrapped[0] = id == TagID.None ? OpenXml(tag) : $"<{tag} identifier={TagIDText(id)}>";
            string closing = tag.Split(' ')[0];
            wrapped[text.Length + 1] = CloseXml(closing);
            Array.Copy(text, 0, wrapped, 1, wrapped.Length);
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
        private static string[] CodeToXml(string contents) => WrapInXml(Code, TagID.CodeSnippet, contents);

        private static (string, string) GetGroupTitleAndDescription(Grouping grouping, List<Documentation> docs)
        {
            List<Documentation> withTitle = docs.Where(doc => doc.GroupInfo.GroupTitle != null).ToList();
            
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
            
            List<Documentation> withDescription = docs.Where(doc => doc.GroupInfo.GroupDescription != null).ToList();
            
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
    }
}