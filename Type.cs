namespace NEWSApi
{
    /// <summary>
    /// Represents a measurement type with a name, description, and associated ranges.
    /// </summary>
    public class Type
    {
        /// <summary>
        /// Initializes a new instance of the "Type" class.
        /// </summary>
        /// <param name="name">The name of the measurement type.</param>
        /// <param name="description">The description of the measurement type.</param>
        /// <param name="ranges">The list of ranges associated with the measurement type.</param>
        public Type(string name, string description, List<Range> ranges)
        {
            // Set the properties of the Type based on the provided parameters.
            Name = name;
            Description = description;
            Ranges = ranges;

            // Calculate and set the minimum and maximum values based on the ranges.
            MinValue = ranges.Select(p => p.Start).Min();
            MaxValue = ranges.Select(p => p.End).Max();
        }

        /// <summary>
        /// Gets or sets the maximum value allowed for the measurement type.
        /// </summary>
        public int MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the minimum value allowed for the measurement type.
        /// </summary>
        public int MinValue { get; set; }

        /// <summary>
        /// Gets or sets the name of the measurement type.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the measurement type.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the list of ranges associated with the measurement type.
        /// </summary>
        public List<Range> Ranges { get; set; }
    }

}
