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
        public char[] Separators { get; set; }

        /// <summary>
        /// Builds a chain of tokenizers configured as defined.
        /// </summary>
        /// <returns>First tokenizer of newly constructed chain.</returns>
        public IArgumentTokenizer BuildTokenizer()
        {
            var shortNameTokenizer = new ShortNameOptionTokenizer();
            if (AllowShortOptionGroups)
                shortNameTokenizer.AddLink(new OptionsGroupTokenizer());
            shortNameTokenizer.AddLink(new LongNameOptionTokenizer());
            if (Separators.Length > 0)
                shortNameTokenizer.AddLink(new ValuesGroupTokenizer(Separators));
            return shortNameTokenizer;
        }
    }
}