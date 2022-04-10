using System;
using System.Collections.Generic;
using System.Reflection;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct GroupInfo : IEqualityComparer<GroupInfo>
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

        public bool Equals(GroupInfo x, GroupInfo y)
        {
            return x.Group == y.Group &&
                   x.IndexInGroup == y.IndexInGroup &&
                   x.GroupTitle == y.GroupTitle &&
                   x.GroupDescription == y.GroupDescription &&
                   Equals(x.MemberBeingDocumented, y.MemberBeingDocumented);
        }

        public int GetHashCode(GroupInfo obj)
        {
            unchecked
            {
                var hashCode = (int)obj.Group;
                hashCode = (hashCode * 397) ^ (int)obj.IndexInGroup;
                hashCode = (hashCode * 397) ^ (obj.GroupTitle != null ? obj.GroupTitle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.GroupDescription != null ? obj.GroupDescription.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.MemberBeingDocumented != null ? obj.MemberBeingDocumented.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}