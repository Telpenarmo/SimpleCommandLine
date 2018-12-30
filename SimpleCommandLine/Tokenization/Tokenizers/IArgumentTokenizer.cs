namespace SimpleCommandLine.Tokenization.Tokenizers
{
    /// <summary>
    /// Declares a method for creating tokens.
    /// </summary>
    public interface IArgumentTokenizer
    {
        /// <summary>
        /// Tokenizes the provided argument.
        /// </summary>
        /// <param name="arg">A value to be tokenized.</param>
        /// <returns>Constructed token.</returns>
        Tokens.IArgumentToken TokenizeArgument(string arg);
    }
}