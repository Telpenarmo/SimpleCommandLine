using System;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class SingleValueParser : IArgumentParser
    {
        protected readonly ArgumentInfo argumentInfo;
        protected readonly ISingleValueConverter valueConverter;
        protected readonly IFormatProvider formatProvider;
        protected ParsingResult result;

        public SingleValueParser(ArgumentInfo argumentInfo, ISingleValueConverter valueConverter, IFormatProvider formatProvider)
        {
            this.argumentInfo = argumentInfo;
            this.valueConverter = valueConverter;
            this.formatProvider = formatProvider;
            result = valueConverter.DefaultValue;
        }

        public virtual bool AcceptsValue => result == null;
        public virtual bool RequiresValue => result == null;

        public void AddValue(ValueToken token) => SetValue(token);

        public void SetValue(ValueToken token)
        {
            if (!AcceptsValue) throw new InvalidOperationException("Value already set.");
            result = valueConverter.Convert(token.Value, formatProvider);
        }

        public virtual ParsingResult Parse(object target)
        {
            if (RequiresValue) throw new InvalidOperationException("Value not set.");
            if (result.IsError) return result;
            argumentInfo.SetValue(target, result.ResultObject);
            return null;
        }
    }
}