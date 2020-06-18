using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal interface IArgumentParser
    {
        void AddValue(ValueToken token);
        ParsingResult Parse(object target);
        bool RequiresValue { get; }
        bool AcceptsValue { get; }
    }
}