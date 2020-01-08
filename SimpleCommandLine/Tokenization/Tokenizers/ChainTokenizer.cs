using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers
{
    /// <summary>
    /// Base class for tokenizers that form chain of responsibility.
    /// </summary>
    public abstract class ChainTokenizer : IArgumentTokenizer
    {
        /// <summary>
        /// Gets or sets the next tokenizer in chain.
        /// </summary>
        public IArgumentTokenizer Next { get; set; } = new ValueTokenizer();

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
            else return Next.TokenizeArgument(arg);
        }

        public void AddLink(ChainTokenizer link)
        {
            if (link is null)
                return;
            var current = this;
            while (current.Next is ChainTokenizer next)
                current = next;
            link.Next = current.Next;
            current.Next = link;
        }

        public abstract bool CanHandle(string arg);
        public abstract IArgumentToken Handle(string arg);
    }
}