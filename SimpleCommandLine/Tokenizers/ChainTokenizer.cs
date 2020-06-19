using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tokenizers
{
    /// <summary>
    /// Base class for tokenizers that form chain of responsibility.
    /// </summary>
    public abstract class ChainTokenizer : IArgumentTokenizer
    {
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
            if (string.IsNullOrWhiteSpace(arg)) return null;
            if (CanHandle(arg)) return Handle(arg);
            else return Next.TokenizeArgument(arg);
        }

        /// <summary>
        /// Checks if this tokenizer is able to handle given argument.
        /// </summary>
        /// <param name="arg">An argument to check.</param>
        public abstract bool CanHandle(string arg);

        /// <summary>
        /// Tokenizes given argument assuming its correctness.
        /// </summary>
        /// <param name="arg">An argument checked by <see cref="CanHandle(string)"/> method.</param>
        public abstract IArgumentToken Handle(string arg);
    }
}