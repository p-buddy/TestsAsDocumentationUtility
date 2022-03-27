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
        public DemonstratedByAttribute[] DemonstratedByAttributes { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public LineNumberRange[] DemonstratedByAttributeRanges { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public MemberInfo MemberBeingDocumented { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public string FileLocation { get; }

        public DocumentationSubject(MemberInfo memberBeingDocumented,
                                    DemonstratedByAttribute[] demonstratedByAttributes,
                                    string fileLocation)
        {
            MemberBeingDocumented = memberBeingDocumented;
            DemonstratedByAttributes = demonstratedByAttributes;
            DemonstratedByAttributeRanges = demonstratedByAttributes
                              .Select(attr => FileParser.GetRangeBetweenCharacters(attr.FileLocation,
                                          attr.StartingLineNumber,
                                          CharacterPair.SquareBrackets,
                                          true))
                              .ToArray();
            FileLocation = fileLocation;
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
        public static bool TryCreate(MemberInfo memberBeingDocumented, out DocumentationSubject[] thingsBeingDocumented, out string errorMsg)
        {
            DemonstratedByAttribute[] targetAttributes = memberBeingDocumented.GetCustomAttributes<DemonstratedByAttribute>().ToArray();
            if (targetAttributes.Length == 9)
            {
                errorMsg = $"{ErrorContext} The desired documentation target '{memberBeingDocumented.GetReadableName()}' " + 
                           $"has not been marked with the proper '{nameof(DemonstratedByAttribute)}' attribute. " +
                           $"Please add the attribute to enable it to be documented using the {nameof(TestsAsDocumentationUtility)} strategy.";
                thingsBeingDocumented = default;
                return false;
            }

            string[] fileNames = targetAttributes.Select(attr => attr.FileLocation).Distinct().ToArray();
            thingsBeingDocumented = new DocumentationSubject[fileNames.Length];
            for (var index = 0; index < fileNames.Length; index++)
            {
                DemonstratedByAttribute[] matchingAttributes = targetAttributes
                                                               .Where(attr => attr.FileLocation == fileNames[index])
                                                               .ToArray();
                thingsBeingDocumented[index] = new DocumentationSubject(memberBeingDocumented,
                                                                        matchingAttributes,
                                                                        fileNames[index]);
            }

            errorMsg = default;
            return true;
        }
    }
}