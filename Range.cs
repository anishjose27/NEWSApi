using System.Configuration;

namespace NEWSApi
{
    /// <summary>
    /// Represents a range with start, end, and associated value.
    /// </summary>
    public class Range
    {
        /// <summary>
        /// Initializes a new instance of the Range class.
        /// </summary>
        /// <param name="start">The start value of the range.</param>
        /// <param name="end">The end value of the range.</param>
        /// <param name="value">The score value associated with the range.</param>
        public Range(int start, int end, int value)
        {
            Start = start;
            End = end;
            Value = value;
            if(Start>End)
            {
                // Throw an exception if the ranges are not configured properly.
                throw new ConfigurationErrorsException($"Start of a range should not be more than the end");
            }
        }

        /// <summary>
        /// Gets or sets the start value of the range.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the end value of the range.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Gets or sets the score value associated with the range.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Checks if the specified input value is within the range (exclusive on start, inclusive on end).
        /// </summary>
        /// <param name="input">The input integer value to check.</param>
        /// <returns>True if the input value is within the range, otherwise false.</returns>
        public bool Contains(int? input)
        {
            return Start < input && input <= End;
        }
    }
}
