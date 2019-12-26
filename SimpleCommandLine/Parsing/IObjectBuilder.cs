using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal interface IObjectBuilder
    {
        bool AwaitsValue { get; }
        IOptionParser LastOption { get; }

        void AddOption(IOptionToken token);
        void AddValue(IValueToken token);
        object Parse();
    }
}