using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers
{
    /// <summary>
    /// Base class for tokenizers that form chain of responsibility.
    /// </summary>
    public abstract class ChainTokenizer : IArgumentTokenizer
    {
        protected IArgumentTokenizer DefaultTokenizer { get; } = new ValueTokenizer();

        /// <summary>
        /// Gets or sets the next tokenizer in chain.
        /// </summary>
        public IArgumentTokenizer Next { get; set; }

        /// <summary>
        /// Tokenizes the provided argument using the current instance or the <see cref="Next"/> tokenizer.
        /// </summary>
        /// <param name="arg">A value to be tokenized.</param>
        /// <returns>A constructed token.</returns>
        public IArgumentToken TokenizeArgument(string arg)
        {
            if (string.IsNullOrWhiteSpace(arg))
                return null;
            if (CanHandle(arg)) return Handle(arg);
            else return Next?.TokenizeArgument(arg) ?? DefaultTokenizer.TokenizeArgument(arg);
        }

        protected abstract bool CanHandle(string arg);
        protected abstract IArgumentToken Handle(string arg);
    }
}