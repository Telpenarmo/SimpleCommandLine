using System;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class ImplicitOptionParser : SingleValueOptionParser
    {
        public ImplicitOptionParser(ParsingArgumentInfo argumentInfo, IValueConverter valueConverter, IOptionToken optionToken)
            : base(argumentInfo, valueConverter, optionToken) { }

        public override bool RequiresValue => false;
        public override bool AcceptsValue => false;
        public override void Parse(object target, IFormatProvider formatProvider)
        {
            if (valueToken == null)
                argumentInfo.SetValue(target, true);
            else
                base.Parse(target, formatProvider);
        }
    }
}