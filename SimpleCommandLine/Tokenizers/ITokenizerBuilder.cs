namespace SimpleCommandLine.Tokenizers
{
    /// <summary>
    /// Declares methods for configuring and building a tokenizer.
    /// </summary>
    public interface ITokenizerBuilder
    {
        /// <summary>
        /// Indicates whether options may be bundled in groups.
        /// </summary>
        public bool AllowShortOptionGroups { get; set; }

        /// <summary>
        /// Build a tokenizer configured as defined.
        /// </summary>
        /// <returns></returns>
        IArgumentTokenizer BuildTokenizer();
    }
}