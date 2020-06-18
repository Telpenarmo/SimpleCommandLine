using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tokenizers
{
    /// <summary>
    /// Builds the <see cref="ValueToken"/>.
    /// </summary>
    public class ValueTokenizer : IArgumentTokenizer
    {
        /// <summary>
        /// Creates a <see cref="ValueToken"/>.
        /// </summary>
        /// <param name="arg">A value argument to be tokenized.</param>
        /// <returns>Constucted token.</returns>
        public IArgumentToken TokenizeArgument(string arg) => new ValueToken(arg);
    }
}