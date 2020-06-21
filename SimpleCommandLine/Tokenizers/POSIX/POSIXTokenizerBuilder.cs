namespace SimpleCommandLine.Tokenizers.POSIX
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
            var valueTokenizer = new ValueTokenizer();
            var shortOptionTokenizer = new ShortOptionTokenizer();
            var longOptionTokenizer = new LongOptionTokenizer();

            shortOptionTokenizer.Next = longOptionTokenizer;
            if (AllowShortOptionGroups)
                longOptionTokenizer.Next = new OptionsGroupTokenizer() { Next = valueTokenizer };
            else
                longOptionTokenizer.Next = valueTokenizer;

            return shortOptionTokenizer;
        }
    }
}