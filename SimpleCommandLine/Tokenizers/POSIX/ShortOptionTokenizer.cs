using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tokenizers.POSIX
{
    internal class ShortOptionTokenizer : ChainTokenizer, IOptionTokenizer
    {
        public bool IsOption(string arg) => arg[0] == '-' && arg.Length == 2;
        public OptionToken ProduceOptionToken(string arg) => new OptionToken(arg[1].ToString());

        public override bool CanHandle(string arg) =>IsOption(arg);
        public override IArgumentToken Handle(string arg) => ProduceOptionToken(arg);
    }
}