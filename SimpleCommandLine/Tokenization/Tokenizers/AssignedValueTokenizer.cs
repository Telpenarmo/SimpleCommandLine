using System;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers
{
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

        public override bool CanHandle(string arg)
        {
            var index = arg.IndexOfAny(separators);
            return index < 1 ? false : optionTokenizer.CanHandle(arg.Substring(0, index));
        }

        public override IArgumentToken Handle(string arg)
        {
            var index = arg.IndexOfAny(separators);
            return new AssignedValueToken(
                optionTokenizer.Handle(arg.Substring(0, index)) as IOptionToken,
                valueTokenizer.TokenizeArgument(arg.Substring(index+1)) as IValueToken);
        }
    }
}