using System;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class SingleValueParser : IArgumentParser
    {
        protected readonly ArgumentInfo argumentInfo;
        protected readonly IValueConverter valueConverter;
        protected readonly IFormatProvider formatProvider;
        protected object value;

        public SingleValueParser(ArgumentInfo argumentInfo, IValueConverter valueConverter, IFormatProvider formatProvider)
        {
            this.argumentInfo = argumentInfo;
            this.valueConverter = valueConverter;
            this.formatProvider = formatProvider;
            valueConverter.Convert(string.Empty, formatProvider, out value); // for implilict options
        }

        public virtual bool AcceptsValue => value == null;
        public virtual bool RequiresValue => value == null;

        public void AddValue(ValueToken valueToken)
        {
            if (!valueConverter.Convert(valueToken.Value, formatProvider, out value))
                throw new ArgumentException();
        }

        public virtual void Parse(object target)
            => argumentInfo.SetValue(target, value);
    }
}