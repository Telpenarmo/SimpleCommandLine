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
        public bool AllowAssigningOptions { get; set; }

        /// <summary>
        /// Indicates whether options may be bundled in groups.
        /// </summary>
        public bool AllowShortOptionGroups { get; set; }

        /// <summary>
        /// Defines characters that separate parts of command-line arguments.
        /// Default is empty (values grouping and assigning not allowed).
        /// </summary>
        public char[] Separators { get; set; } = System.Array.Empty<char>();

        /// <summary>
        /// Builds a chain of tokenizers configured as defined.
        /// </summary>
        /// <returns>First tokenizer of newly constructed chain.</returns>
        public IArgumentTokenizer BuildTokenizer()
        {
            var shortNameTokenizer = new ShortNameOptionTokenizer();
            var valueTokenizer = Separators.Length == 0
                ? new ValueTokenizer() as IArgumentTokenizer
                : new ValuesGroupTokenizer(Separators, new ValueTokenizer());

            if (AllowAssigningOptions)
            {
                shortNameTokenizer.AppendLink(new AssignedValueTokenizer(Separators, new ShortNameOptionTokenizer(), valueTokenizer));               
                shortNameTokenizer.AppendLink(new AssignedValueTokenizer(Separators, new LongNameOptionTokenizer(), valueTokenizer));
            }
            if (AllowShortOptionGroups)
                shortNameTokenizer.AppendLink(new OptionsGroupTokenizer());
            shortNameTokenizer.AppendLink(new LongNameOptionTokenizer(){ Next = valueTokenizer });
            
            return shortNameTokenizer;
        }
    }
}