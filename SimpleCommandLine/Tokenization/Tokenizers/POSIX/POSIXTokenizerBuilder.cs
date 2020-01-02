namespace SimpleCommandLine.Tokenization.Tokenizers.POSIX
{
    /// <summary>
    /// Used to build a chain of tokenizers using POSIX standard.
    /// </summary>
    public class POSIXTokenizerBuilder : ITokenizerBuilder
    {
        /// <summary>
        /// Indicates whether options may be explicitly assigned.
        /// </summary>
        public bool AllowAssigningOptions { get; set; } = false;

        /// <summary>
        /// Indicates whether options may be bundled in groups.
        /// </summary>
        public bool AllowShortOptionGroups { get; set; }

        /// <summary>
        /// Defines characters that separate parts of command-line arguments.
        /// </summary>
        public char[] Separators { get; set; }

        /// <summary>
        /// Builds a chain of tokenizers configured as defined.
        /// </summary>
        /// <returns>First tokenizer of newly constructed chain.</returns>
        public IArgumentTokenizer BuildTokenizer()
        {
            var shortNameTokenizer = new ShortNameOptionTokenizer();
            var valueTokenizer = Separators.Length == 0
                ? new ValueTokenizer() as IArgumentTokenizer
                : new ValuesGroupTokenizer(Separators);

            if (AllowAssigningOptions)
            {
                shortNameTokenizer.AddLink(new AssignedValueTokenizer(Separators, new ShortNameOptionTokenizer(), valueTokenizer));               
                shortNameTokenizer.AddLink(new AssignedValueTokenizer(Separators, new LongNameOptionTokenizer(), valueTokenizer));
            }
            if (AllowShortOptionGroups)
                shortNameTokenizer.AddLink(new OptionsGroupTokenizer());
            shortNameTokenizer.AddLink(new LongNameOptionTokenizer(){ Next = valueTokenizer });
            
            return shortNameTokenizer;
        }
    }
}