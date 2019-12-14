using System;
using System.Linq;
using System.Collections.Generic;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers
{
    public class ValuesGroupTokenizer : ChainTokenizer
    {
        private readonly char[] separators;

        public ValuesGroupTokenizer(char[] separators)
        {
            if (separators.Length == 0)
                throw new ArgumentException("At least one separator must be defined.");
            this.separators = separators;
        }

        protected override bool CanHandle(string arg)
        {
            return separators.Any(s => arg.Contains(s));
        }

        protected override IArgumentToken Handle(string arg)
        {
            return HandleByRecursion(arg.Split(separators[0]), 1);
        }

        private IArgumentToken HandleByRecursion(string[] args, int sepIndex)
        {
            IEnumerable<IArgumentToken> tokens;

            if (separators.Length == sepIndex)
                tokens = args.Select(a => DefaultTokenizer.TokenizeArgument(a));
            else
                tokens = args.Select(arg => HandleByRecursion(arg.Split(separators[sepIndex]), sepIndex+1));

            return tokens.Count() == 1 ? tokens.Single() : new ValuesGroupToken(tokens);
        }
    }
}
