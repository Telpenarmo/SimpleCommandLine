using System;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers
{
    /// <summary>
    /// Builds the <see cref="AssignedValueToken"/>.
    /// </summary>
    public class AssignedValueTokenizer : ChainTokenizer
    {
        private readonly char[] separators;
        private readonly ChainTokenizer optionTokenizer;
        private readonly IArgumentTokenizer valueTokenizer;

        public AssignedValueTokenizer(char[] separators, ChainTokenizer optionTokenizer, IArgumentTokenizer valueTokenizer)
        {
            this.separators = separators ?? throw new ArgumentNullException(nameof(separators));            
            this.optionTokenizer = optionTokenizer ?? throw new ArgumentNullException(nameof(optionTokenizer));
            this.valueTokenizer = valueTokenizer ?? throw new ArgumentNullException(nameof(valueTokenizer));
        }
        
        /// <summary>
        /// Checks if this tokenizer is able to handle given argument.
        /// </summary>
        /// <param name="arg">An argument to check.</param>
        public override bool CanHandle(string arg)
        {
            var index = arg.IndexOfAny(separators);
            return index >= 1 && optionTokenizer.CanHandle(arg.Substring(0, index));
        }

        /// <summary>
        /// Tokenizes given argument assuming its correctness.
        /// </summary>
        /// <param name="arg">An argument checked by <see cref="CanHandle(string)"/> method.</param>
        public override IArgumentToken Handle(string arg)
        {
            var index = arg.IndexOfAny(separators);
            return new AssignedValueToken(
                optionTokenizer.Handle(arg.Substring(0, index)) as IOptionToken,
                valueTokenizer.TokenizeArgument(arg.Substring(index+1)) as IValueToken);
        }
    }
}