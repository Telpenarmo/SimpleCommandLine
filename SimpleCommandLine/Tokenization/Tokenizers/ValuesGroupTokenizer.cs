using System;
using System.Collections.Generic;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers
{
    public class ValuesGroupTokenizer : ChainTokenizer
    {
        private readonly char[] separators;

        public ValuesGroupTokenizer(char[] separators)
        {
            this.separators = separators;
        }

        protected override bool CanHandle(string arg)
        {
            throw new NotImplementedException();
        }

        protected override IArgumentToken Handle(string arg)
        {
            throw new NotImplementedException();
        }
    }
}
