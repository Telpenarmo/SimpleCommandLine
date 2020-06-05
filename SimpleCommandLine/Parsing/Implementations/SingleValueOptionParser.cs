using System;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class SingleValueOptionParser : SingleValueParser, IOptionParser
    {
        public SingleValueOptionParser(ParsingArgumentInfo argumentInfo, IValueConverter valueConverter, OptionToken optionToken)
            : base(argumentInfo, valueConverter)
        {
            OptionToken = optionToken;
        }

        public virtual bool AcceptsValue => valueToken == null;
        public virtual bool RequiresValue => valueToken == null;
        public OptionToken OptionToken { get; }
    }
}
