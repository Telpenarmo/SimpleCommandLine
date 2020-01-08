using System;
using System.Linq;
using System.Collections.Generic;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers
{
    public class ValuesGroupTokenizer : ChainTokenizer
    {
        private readonly char[] separators;
        private readonly ValueTokenizer valueTokenizer;

        public ValuesGroupTokenizer(char[] separators, ValueTokenizer valueTokenizer)
        {
            this.valueTokenizer = valueTokenizer ?? throw new ArgumentNullException(nameof(valueTokenizer));
            if (separators.Length == 0)
                throw new ArgumentException("At least one separator must be defined.");
            this.separators = separators;
        }

        public override bool CanHandle(string arg)
        {
            return separators.Any(s => arg.Contains(s));
        }

        public override IArgumentToken Handle(string arg)
        {
            return HandleRecursively(arg.Split(separators[0]), 1);
        }

        private IValueToken HandleRecursively(string[] args, int sepIndex)
        {
            IEnumerable<IValueToken> tokens;

            if (separators.Length == sepIndex)
                tokens = args.Select(arg => valueTokenizer.TokenizeArgument(arg) as ValueToken);
            else
                tokens = args.Select(arg => HandleRecursively(arg.Split(separators[sepIndex]), sepIndex+1));

            return tokens.Count() == 1 ? tokens.Single() : new ValuesGroupToken(tokens);
        }
    }
}
