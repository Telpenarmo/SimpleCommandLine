using SimpleCommandLine.Tokenizers;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tests.Fakes
{
    public class FakeTokenizer : IArgumentTokenizer
    {
        public bool Invoked { get; private set; }
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
            public bool Equals(IArgumentToken other)
                => ReferenceEquals(this, other);
        }
    }
}
