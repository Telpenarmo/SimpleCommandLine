using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal interface IOptionParser : IArgumentParser
    {
        bool RequiresValue { get; }
        bool AcceptsValue { get; }
        OptionToken OptionToken { get; }
    }
}