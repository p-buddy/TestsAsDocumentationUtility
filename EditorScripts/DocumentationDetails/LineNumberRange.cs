namespace pbuddy.TestsAsDocumentationUtility.EditorScripts
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct LineNumberRange
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
    }
}