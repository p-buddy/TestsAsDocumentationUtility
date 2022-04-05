using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public static class GeneratorUtility
    {
        private const string ErrorContext = "[" + nameof(GeneratorUtility) + " ERROR]: ";
        private static readonly string NoneGroup = Enum.GetName(typeof(Grouping), Grouping.None);
        
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
    }
}