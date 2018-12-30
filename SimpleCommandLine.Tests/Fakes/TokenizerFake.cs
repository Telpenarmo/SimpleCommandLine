using System;
using System.Collections.Generic;
using SimpleCommandLine.Tokenization.Tokenizers;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tests.Fakes
{
    public class TokenizerFake : IArgumentTokenizer
    {
        public bool Invoked { get;private set; }
        public string Argument { get; private set; }
        public IArgumentToken TokenizeArgument(string arg)
        {
            Invoked = true;
            Argument = arg;
            return new TokenFake();
        }

        public void Reset()
        {
            Invoked = false;
            Argument = string.Empty;
        }

        private class TokenFake : IArgumentToken
        {

        }
    }
}
