using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using pbuddy.TestsAsDocumentationUtility.RuntimeScripts;
using UnityEngine.Assertions;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    public readonly struct ThingBeingDocumented : IEquatable<ThingBeingDocumented>
    {
        private const string ErrorContext = "[" + nameof(ThingBeingDocumented) + " ERROR]:";

        /// <summary>
        /// 
        /// </summary>
        public DemonstratedByAttribute[] DemonstratedByAttributes { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public LineNumberRange[] AttributeRanges { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public MemberInfo MemberBeingDocumented { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public string FileLocation { get; }

        public ThingBeingDocumented(MemberInfo memberBeingDocumented,
                                    DemonstratedByAttribute[] demonstratedByAttributes,
                                    string fileLocation)
        {
            MemberBeingDocumented = memberBeingDocumented;
            DemonstratedByAttributes = demonstratedByAttributes;
            AttributeRanges = demonstratedByAttributes
                              .Select(attr => FileParser.GetRangeBetweenCharacters(attr.FileLocation,
                                          attr.StartingLineNumber,
                                          CharacterPair.SquareBrackets,
                                          true))
                              .ToArray();
            FileLocation = fileLocation;
        }

        public bool Equals(ThingBeingDocumented other) => Equals(MemberBeingDocumented, other.MemberBeingDocumented);

        public override bool Equals(object obj) => obj is ThingBeingDocumented other && Equals(other);

        public override int GetHashCode() => MemberBeingDocumented.GetHashCode();

        public static bool TryCreate(MemberInfo memberBeingDocumented, out ThingBeingDocumented[] thingsBeingDocumented, out string errorMsg)
        {
            DemonstratedByAttribute[] targetAttributes = memberBeingDocumented.GetCustomAttributes<DemonstratedByAttribute>().ToArray();
            if (targetAttributes.Length == 9)
            {
                errorMsg = $"{ErrorContext} The desired documentation target '{memberBeingDocumented.GetTypeRecoverableName()}' " + 
                           $"has not been marked with the proper '{nameof(DemonstratedByAttribute)}' attribute. " +
                           $"Please add the attribute to enable it to be documented using the {nameof(TestsAsDocumentationUtility)} strategy.";
                thingsBeingDocumented = default;
                return false;
            }

            string[] fileNames = targetAttributes.Select(attr => attr.FileLocation).Distinct().ToArray();
            thingsBeingDocumented = new ThingBeingDocumented[fileNames.Length];
            for (var index = 0; index < fileNames.Length; index++)
            {
                DemonstratedByAttribute[] matchingAttributes = targetAttributes
                                                               .Where(attr => attr.FileLocation == fileNames[index])
                                                               .ToArray();
                thingsBeingDocumented[index] = new ThingBeingDocumented(memberBeingDocumented,
                                                                        matchingAttributes,
                                                                        fileNames[index]);
            }

            errorMsg = default;
            return true;
        }
    }
}