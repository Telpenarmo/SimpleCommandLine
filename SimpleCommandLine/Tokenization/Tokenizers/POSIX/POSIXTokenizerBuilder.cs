namespace SimpleCommandLine.Tokenization.Tokenizers.POSIX
{
    /// <summary>
    /// Used to build a chain of tokenizers using POSIX standard.
    /// </summary>
    public class POSIXTokenizerBuilder : ITokenizerBuilder
    {
        /// <summary>
        /// Indicates whether options may be bundled in groups.
        /// </summary>
        public bool AllowShortOptionGroups { get; set; }

        /// <summary>
        /// Builds a chain of tokenizers configured as defined.
        /// </summary>
        /// <returns>First tokenizer of newly constructed chain.</returns>
        public IArgumentTokenizer BuildTokenizer()
        {
            var shortNameTokenizer = new ShortNameOptionTokenizer();
            var longNameTokenizer = new LongNameOptionTokenizer();
            if (AllowShortOptionGroups)
                longNameTokenizer.Next = new OptionsGroupTokenizer();
            shortNameTokenizer.Next = longNameTokenizer;
            return shortNameTokenizer;
        }
    }
}