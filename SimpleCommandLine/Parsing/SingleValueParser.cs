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
        protected ParsingResult result;

        public SingleValueParser(ArgumentInfo argumentInfo, IValueConverter valueConverter, IFormatProvider formatProvider)
        {
            this.argumentInfo = argumentInfo;
            this.valueConverter = valueConverter;
            this.formatProvider = formatProvider;
            result = valueConverter.DefaultValue;
        }

        public virtual bool AcceptsValue => result == null;
        public virtual bool RequiresValue => result == null;

        public void AddValue(ValueToken valueToken)
            => result = valueConverter.Convert(valueToken.Value, formatProvider);

        public virtual ParsingResult Parse(object target)
        {
            if (result.IsError) return result;
            argumentInfo.SetValue(target, result);
            return ParsingResult.Success(target);
        }
    }
}