using System;
using System.Collections.Generic;
using System.Reflection;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct GroupInfo : IEqualityComparer<GroupInfo>, IEquatable<GroupInfo>
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

        public bool Equals(GroupInfo other)
        {
            return Group == other.Group &&
                   IndexInGroup == other.IndexInGroup &&
                   GroupTitle == other.GroupTitle &&
                   GroupDescription == other.GroupDescription &&
                   Equals(MemberBeingDocumented, other.MemberBeingDocumented);
        }

        public override bool Equals(object obj)
        {
            return obj is GroupInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Group;
                hashCode = (hashCode * 397) ^ (int)IndexInGroup;
                hashCode = (hashCode * 397) ^ (GroupTitle != null ? GroupTitle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (GroupDescription != null ? GroupDescription.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (MemberBeingDocumented != null ? MemberBeingDocumented.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}