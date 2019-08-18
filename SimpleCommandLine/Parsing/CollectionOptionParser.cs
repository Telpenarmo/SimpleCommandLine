using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class CollectionOptionParser : CollectionParser, IOptionParser
    {
        public CollectionOptionParser(Registration.ParsingArgumentInfo argumentInfo, CollectionConverter converter, IOptionToken optionToken)
            : base(argumentInfo, converter)
        {
            OptionToken = optionToken;
        }

        public IOptionToken OptionToken { get; }
    }
}