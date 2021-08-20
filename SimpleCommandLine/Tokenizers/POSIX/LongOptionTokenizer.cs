using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tokenizers.POSIX
{
    internal class LongOptionTokenizer : ChainTokenizer, IOptionTokenizer
    {
        public bool IsOption(string arg) => arg.StartsWith("--") && arg.Length > 3;
        public OptionToken ProduceOptionToken(string arg) => new(arg.Substring(2));

        public override bool CanHandle(string arg) => IsOption(arg);
        public override IArgumentToken Handle(string arg) => ProduceOptionToken(arg);
    }
}