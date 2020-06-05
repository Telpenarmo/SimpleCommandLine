using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers.POSIX
{
    internal class LongNameOptionTokenizer : ChainTokenizer
    {
        public override bool CanHandle(string arg) => arg.StartsWith("--") && arg.Length > 3;

        public override IArgumentToken Handle(string arg) => new OptionToken(arg.Substring(2));
    }
}