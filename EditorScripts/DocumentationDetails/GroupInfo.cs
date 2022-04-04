using System.Reflection;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct GroupInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public Grouping Group { get; }

        /// <summary>
        /// 
        /// </summary>
        public IndexInGroup IndexInGroup { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public string GroupTitle { get; }

        /// <summary>
        /// 
        /// </summary>
        public string GroupDescription { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public MemberInfo MemberBeingDocumented { get; }
        
        public GroupInfo(Grouping grouping,
                         IndexInGroup indexInGroup,
                         string groupTitle,
                         string groupDescription,
                         MemberInfo memberBeingDocumented)
        {
            Group = grouping;
            IndexInGroup = indexInGroup;
            GroupTitle = groupTitle;
            GroupDescription = groupDescription;
            MemberBeingDocumented = memberBeingDocumented;
        }
    }
}