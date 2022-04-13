using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct DocumentationEntry
    {
        private const string GroupTitle = nameof(GroupInfo.GroupTitle);
        private const string GroupDescription = nameof(GroupInfo.GroupDescription);
        
        public string Title { get; }
        public string Description { get; }
        
        public CodeBlock[] Blocks { get; }

        public DocumentationEntry(in DocumentationGroup group, List<IMarkup> markups)
        {
            List<DocumentationSnippet> snippets = group.GetSnippets();
            Blocks = snippets.Select(snip => snip.GetCodeBlock(markups)).ToArray();

            if (group.Group != Grouping.None)
            {
                (Title, Description) = GetEntryTitleAndDescription(snippets);
                return;
            }
            
            Assert.AreEqual(snippets.Count, 1);
            (Title, Description) = GetEntryTitleAndDescription(snippets[0]);
            Blocks[0] = Blocks[0].NullifyTitleAndDescription();
        }

        private static (string, string) GetEntryTitleAndDescription(in DocumentationSnippet snippet)
        {
            if (snippet.GroupInfo.Group == Grouping.None)
            {
                Assert.IsNull(snippet.GroupInfo.GroupTitle, GetUnusedGroupInfoErrorMsg(in snippet, GroupTitle));
                Assert.IsNull(snippet.GroupInfo.GroupDescription, GetUnusedGroupInfoErrorMsg(in snippet, GroupDescription));
                return (snippet.Title, snippet.Description);
            }
            
            return (snippet.GroupInfo.GroupTitle, snippet.GroupInfo.GroupDescription);

            #region Local Function(s)
            string GetUnusedGroupInfoErrorMsg(in DocumentationSnippet doc, string infoType)  => 
                nameof(GetEntryTitleAndDescription).ErrorContext<DocumentationEntry>(true) + 
                $"Documenting members in the {Grouping.None} should NOT provide {infoType.DelimitCamelCase()}s. " +
                $"{doc.MemberDoingTheDocumenting.GetReadableName()} does " + 
                $"(while documenting {doc.MemberBeingDocumented.GetReadableName()}).";
            #endregion
        }
        
        private static (string, string) GetEntryTitleAndDescription(List<DocumentationSnippet> snippets)
        {
            List<DocumentationSnippet> withTitle = snippets.Where(HasTitle).ToList();
            Assert.IsTrue(withTitle.Count <= 1, GetConflictingInfoError(withTitle, GroupTitle));
            
            List<DocumentationSnippet> withDescription = snippets.Where(HasDescription).ToList();
            Assert.IsTrue(withDescription.Count <= 1, GetConflictingInfoError(withDescription, GroupDescription)); 

            string title = withTitle.Count == 1 ? withTitle[0].GroupInfo.GroupTitle : null;
            string description = withDescription.Count == 1 ? withDescription[0].GroupInfo.GroupDescription : null;
            
            return (title, description);
            
            #region Local Function(s)
            static bool HasTitle(DocumentationSnippet doc) => doc.GroupInfo.GroupTitle != null;
            static bool HasDescription(DocumentationSnippet doc) => doc.GroupInfo.GroupDescription != null;
            static string GetMemberName(DocumentationSnippet doc) => doc.MemberDoingTheDocumenting.GetReadableName();

            static string GetConflictingInfoError(List<DocumentationSnippet> withInfo, string infoType) =>
                nameof(GetEntryTitleAndDescription).ErrorContext<DocumentationEntry>(true) +
                $"Only ONE documenting member in a group should provide a {infoType.DelimitCamelCase()}. " +
                $"All of the following members documenting {withInfo[0].MemberBeingDocumented.GetReadableName()} " + 
                $"provided a {infoType.DelimitCamelCase()}:{Environment.NewLine}" + 
                $"{String.Join(Environment.NewLine, withInfo.Select(GetMemberName)) }";  
            #endregion
        }
    }
}