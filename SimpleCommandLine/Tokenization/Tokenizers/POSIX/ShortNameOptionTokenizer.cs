using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers.POSIX
{
    internal class ShortNameOptionTokenizer : ChainTokenizer
    {
        public override bool CanHandle(string arg) => arg[0] == '-' && arg.Length == 2;

        public override IArgumentToken Handle(string arg) => new ShortOptionToken(arg[1]);
    }
}