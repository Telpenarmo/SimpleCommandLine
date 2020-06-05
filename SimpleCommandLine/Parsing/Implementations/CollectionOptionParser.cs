using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class CollectionOptionParser : CollectionParser, IOptionParser
    {
        public CollectionOptionParser(ParsingArgumentInfo argumentInfo, CollectionConverter converter, OptionToken optionToken)
            : base(argumentInfo, converter)
        {
            OptionToken = optionToken;
        }

        public OptionToken OptionToken { get; }
    }
}