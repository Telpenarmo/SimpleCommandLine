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
        /// Defines characters that separate parts of command-line arguments.
        /// Default is empty (values grouping and assigning not allowed).
        /// </summary>
        public char[] Separators { get; set; } = new[] { '=' };

        /// <summary>
        /// Builds a chain of tokenizers configured as defined.
        /// </summary>
        /// <returns>First tokenizer of newly constructed chain.</returns>
        public IArgumentTokenizer BuildTokenizer()
        {
            var shortOptionTokenizer = new ShortOptionTokenizer();
            var longOptionTokenizer = new LongOptionTokenizer();
            var optionsGroupTokenizer = new OptionsGroupTokenizer();
            var valueTokenizer = new ValueTokenizer();
            var sAssignedValueTokenizer
                = new AssignedValueTokenizer(Separators, new ShortOptionTokenizer(), valueTokenizer);
            var lAssignedValueTokenizer
                = new AssignedValueTokenizer(Separators, new LongOptionTokenizer(), valueTokenizer);

            lAssignedValueTokenizer.Next = sAssignedValueTokenizer;
            sAssignedValueTokenizer.Next = optionsGroupTokenizer;
            optionsGroupTokenizer.Next = longOptionTokenizer;
            longOptionTokenizer.Next = shortOptionTokenizer;
            shortOptionTokenizer.Next = valueTokenizer;
            
            return lAssignedValueTokenizer;
        }
    }
}