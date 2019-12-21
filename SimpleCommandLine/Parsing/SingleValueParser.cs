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
            this.argumentInfo = argumentInfo ?? throw new ArgumentNullException(nameof(argumentInfo));
            this.valueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));
        }

        public void AddValue(IValueToken valueToken)
        {
            if (this.valueToken is ValueToken value)
                this.valueToken = value;
            else throw new ArgumentException();
        }

        public virtual void Parse(object target, IFormatProvider formatProvider)
            => argumentInfo.SetValue(target, valueConverter.Convert(valueToken.Value, formatProvider));
    }
}
