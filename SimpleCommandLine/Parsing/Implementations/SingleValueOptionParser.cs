using System;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class SingleValueOptionParser : SingleValueParser, IOptionParser
    {
        public SingleValueOptionParser(ParsingArgumentInfo argumentInfo, IValueConverter valueConverter, IOptionToken optionToken)
            : base(argumentInfo, valueConverter)
        {
            OptionToken = optionToken;
        }

        public virtual bool AcceptsValue => valueToken == null;
        public virtual bool RequiresValue => valueToken == null;
        public IOptionToken OptionToken { get; }
    }
}
