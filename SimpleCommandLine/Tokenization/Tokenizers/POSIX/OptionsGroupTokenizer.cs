using System.Linq;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers.POSIX
{
    internal class OptionsGroupTokenizer : ChainTokenizer
    {
        public override bool CanHandle(string arg) => arg.Length > 2 && arg[0] == '-' && arg[1] != '-';

        public override IArgumentToken Handle(string arg)
            => new OptionsGroupToken(arg.Skip(1).Select(x => new ShortOptionToken(x)));
    }
}
