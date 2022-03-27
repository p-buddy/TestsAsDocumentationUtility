using System.Linq;
using System.Reflection;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct Keyword
    {
        public string Label { get; }
        public MemberTypes ApplicableMemberTypes { get; }
        
        public Keyword(string label, params MemberTypes[] applicableMemberTypes)
        {
            Label = label;
            ApplicableMemberTypes = applicableMemberTypes.Aggregate((x, y) => x | y);
        }
    }
}