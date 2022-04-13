using System;
using System.Reflection;
using NUnit.Framework;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct AppliesTo
    {
        private const string ErrorContext = "[" + nameof(AppliesTo) + " ERROR]: ";

        private readonly MemberInfo member;
        private readonly Grouping? grouping;
        private readonly IndexInGroup? indexInGroup;
        
        public AppliesTo(Type typeOfThing,
                         Grouping? grouping = null,
                         IndexInGroup? indexInGroup = null)
        {
            member = typeOfThing;
            this.grouping = grouping;
            this.indexInGroup = indexInGroup;
        }

        public AppliesTo(Type typeOfThing,
                         string memberName,
                         Grouping? grouping = null,
                         IndexInGroup? indexInGroup = null)
        {
            bool result = typeOfThing.TryGetNonOverloadedMember(memberName, out member, out string error);
            Assert.IsTrue(result, $"{ErrorContext}: {error}");
            this.grouping = grouping;
            this.indexInGroup = indexInGroup;
        }
        
        public AppliesTo(Type typeOfThing,
                         string memberName,
                         Type[] argumentTypes,
                         Grouping? grouping = null,
                         IndexInGroup? indexInGroup = null)
        {
            bool result = typeOfThing.TryGetOverloadedMember(memberName, argumentTypes, out member, out string error);
            Assert.IsTrue(result, $"{ErrorContext}: {error}");
            this.grouping = grouping;
            this.indexInGroup = indexInGroup;
        }

        public bool Test(in DocumentationSnippet snippet)
        {
            if (snippet.MemberBeingDocumented != member)
            {
                return false;
            }

            if (!grouping.HasValue && !indexInGroup.HasValue)
            {
                return true;
            }

            if (grouping.HasValue && grouping.Value == snippet.Group && !indexInGroup.HasValue)
            {
                return true;
            }

            if (indexInGroup.HasValue && indexInGroup.Value == snippet.GroupInfo.IndexInGroup)
            {
                return true;
            }

            return false;
        }
    }
}