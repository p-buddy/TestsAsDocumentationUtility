using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;
using UnityEngine.Assertions;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct DocumentationSubject : IEquatable<DocumentationSubject>
    {
        private const string ErrorContext = "[" + nameof(DocumentationSubject) + " ERROR]:";

        /// <summary>
        /// 
        /// </summary>
        public IsDemonstratedByTestsAttribute Attribute { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public LineNumberRange AttributeRange { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public MemberInfo MemberBeingDocumented { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public string FileLocation { get; }

        public DocumentationSubject(MemberInfo memberBeingDocumented, IsDemonstratedByTestsAttribute attribute)
        {
            MemberBeingDocumented = memberBeingDocumented;
            Attribute = attribute;
            FileLocation = attribute.FileLocation;
            AttributeRange = FileParser.GetLineNumberRangeForAttribute(attribute.StartingLineNumber, FileLocation);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(DocumentationSubject other) =>
            Equals(MemberBeingDocumented, other.MemberBeingDocumented) &&
            Equals(FileLocation, other.FileLocation);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is DocumentationSubject other && Equals(other);
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => FileLocation.GetHashCode();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberBeingDocumented"></param>
        /// <param name="thingsBeingDocumented"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        public static bool TryCreate(MemberInfo memberBeingDocumented, out DocumentationSubject thingsBeingDocumented, out string errorMsg)
        {
            var targetAttribute = memberBeingDocumented.GetCustomAttribute<IsDemonstratedByTestsAttribute>();
            if (targetAttribute == null)
            {
                errorMsg = $"{ErrorContext} The desired documentation target '{memberBeingDocumented.GetReadableName()}' " + 
                           $"has not been marked with the proper '{nameof(IsDemonstratedByTestsAttribute)}' attribute. " +
                           $"Please manually add the attribute to enable it to be documented using the {nameof(TestsAsDocumentationUtility)} strategy.";
                thingsBeingDocumented = default;
                return false;
            }

            thingsBeingDocumented = new DocumentationSubject(memberBeingDocumented, targetAttribute);

            errorMsg = default;
            return true;
        }
    }
}