using System;
using System.Collections.Generic;

namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct LineNumberRange : IEquatable<LineNumberRange>
    {
        /// <summary>
        /// 
        /// </summary>
        public int Start { get; }
        
        /// <summary>
        /// 
        /// </summary>
        public int End { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>

        public LineNumberRange(int start, int end)
        {
            Start = start;
            End = end;
        }

        public bool Equals(LineNumberRange other)
        {
            return Start == other.Start && End == other.End;
        }

        public override bool Equals(object obj)
        {
            return obj is LineNumberRange other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start * 397) ^ End;
            }
        }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }
    }
}