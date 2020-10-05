using System;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tokenizers
{
    /// <summary>
    /// Builds the <see cref="AssignedValueToken"/>.
    /// </summary>
    public class AssignedValueTokenizer : ChainTokenizer
    {
        private readonly char[] separators;
        private readonly IOptionTokenizer optionTokenizer;
        private readonly IValueTokenizer valueTokenizer;

        public AssignedValueTokenizer(char[] separators, IOptionTokenizer optionTokenizer, IValueTokenizer valueTokenizer)
        {
            this.separators = separators;            
            this.optionTokenizer = optionTokenizer;
            this.valueTokenizer = valueTokenizer;
        }
        
        /// <summary>
        /// Checks if this tokenizer is able to handle given argument.
        /// </summary>
        /// <param name="arg">An argument to check.</param>
        public override bool CanHandle(string arg)
        {
            var index = arg.IndexOfAny(separators);
            return index >= 1 && optionTokenizer.IsOption(arg.Substring(0, index));
        }

        /// <summary>
        /// Tokenizes given argument assuming its correctness.
        /// </summary>
        /// <param name="arg">An argument checked by <see cref="CanHandle(string)"/> method.</param>
        public override IArgumentToken Handle(string arg)
        {
            var index = arg.IndexOfAny(separators);
            return new AssignedValueToken(
                optionTokenizer.ProduceOptionToken(arg.Substring(0, index)),
                valueTokenizer.ProduceValueToken(arg.Substring(index+1)));
        }
    }
}	