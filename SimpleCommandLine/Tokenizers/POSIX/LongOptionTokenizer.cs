using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tokenizers.POSIX
{
    internal class LongOptionTokenizer : ChainTokenizer
    {
        public override bool CanHandle(string arg) => arg.StartsWith("--") && arg.Length > 3;

        public override IArgumentToken Handle(string arg) => new OptionToken(arg.Substring(2));
    }
}