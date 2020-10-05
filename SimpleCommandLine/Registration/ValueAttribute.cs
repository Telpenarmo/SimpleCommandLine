namespace SimpleCommandLine
{
    /// <summary>
    /// Marks a property as a command-line value.
    /// </summary>
    public class ValueAttribute : ParameterAttribute
    {
        /// <summary>
        /// Initializes the <see cref="ValueAttribute"/> with a specified index.
        /// </summary>
        /// <param name="index">Number of this value in the sequence of values in current type.</param>
        public ValueAttribute(uint index) => Index = index;

        /// <summary>
        /// Gets number of this value in the sequence of values in current type.
        /// </summary>
        public uint Index { get; }
    }
}