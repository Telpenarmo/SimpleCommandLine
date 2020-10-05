using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal interface IArgumentHandler
    {
        void AddValue(ValueToken token);
        void SetValue(ValueToken token);
        ParsingResult GetResult();
        bool RequiresValue { get; }
        bool AcceptsValue { get; }
        ParameterInfo ParameterInfo { get; }
    }
}