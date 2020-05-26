using System;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class SingleValueParser : IArgumentParser
    {
        protected readonly ParsingArgumentInfo argumentInfo;
        protected readonly IValueConverter valueConverter;
        protected ValueToken valueToken;

        public SingleValueParser(ParsingArgumentInfo argumentInfo, IValueConverter valueConverter)
        {
            this.argumentInfo = argumentInfo;
            this.valueConverter = valueConverter;
        }

        public void AddValue(ValueToken valueToken)
        {
            if (this.valueToken == null)
                this.valueToken = valueToken;
            else throw new ArgumentException();
        }

        public virtual void Parse(object target, IFormatProvider formatProvider)
            => argumentInfo.SetValue(target, valueConverter.Convert(valueToken.Value, formatProvider));
    }
}
