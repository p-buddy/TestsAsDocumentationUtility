using System.Linq;
using System.Reflection;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts.Declarations
{
    public readonly struct Keyword
    {
        public string Text { get; }
        public MemberTypes ApplicableMemberTypes { get; }
        
        public Keyword(string text, params MemberTypes[] applicableMemberTypes)
        {
            Text = text;
            ApplicableMemberTypes = applicableMemberTypes.Aggregate((x, y) => x | y);
        }
        
        public static Keyword Required()
    }
}