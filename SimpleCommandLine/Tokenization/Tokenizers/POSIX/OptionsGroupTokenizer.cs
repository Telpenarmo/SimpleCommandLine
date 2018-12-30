using System.Linq;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers.POSIX
{
    internal class OptionsGroupTokenizer : ChainTokenizer
    {
        protected override bool CanHandle(string arg) => arg.Length > 2 && arg[0] == '-' && arg[1] != '-';

        protected override IArgumentToken Handle(string arg) => new OptionsGroupToken(arg.Skip(1).Select(x => new ShortOptionToken(x)));
    }
}
