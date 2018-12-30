using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal interface IObjectBuilder
    {
        object Build();
        void SetBoundValue(IOptionToken token, string value);
        void SetUnboundValue(string value);
        void HandleImplicitOption(IOptionToken token);
    }
}