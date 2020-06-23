using System.Linq;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tokenizers.POSIX
{
    internal class OptionsGroupTokenizer : ChainTokenizer
    {
        public override bool CanHandle(string arg)
            => arg.Length > 2 && arg[0] == '-' && ! arg.Skip(1).Contains('-');

        public override IArgumentToken Handle(string arg)
            => new OptionsGroupToken(arg.Skip(1).Select(x => new OptionToken(x.ToString())));
    }
}