using System;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class UnaryParser : IArgumentParser
    {
        protected readonly ArgumentInfo argumentInfo;
        protected readonly ISingleValueConverter valueConverter;
        protected readonly IFormatProvider formatProvider;
        protected ParsingResult result;

        public UnaryParser(ArgumentInfo argumentInfo, ISingleValueConverter valueConverter, IFormatProvider formatProvider)
        {
            this.argumentInfo = argumentInfo;
            this.valueConverter = valueConverter;
            this.formatProvider = formatProvider;
            var def = valueConverter.DefaultValue;
            if (def != null) result = ParsingResult.Success(def);
        }

        public virtual bool AcceptsValue => result == null;
        public virtual bool RequiresValue => result == null;

        public void AddValue(ValueToken token)
        {
            if (!AcceptsValue) throw new InvalidOperationException("Value already set.");
            SetValue(token);
        }

        public void SetValue(ValueToken token)
        {
            result = valueConverter.Convert(token.Value, formatProvider);
        }

        public virtual ParsingResult Parse(object target)
        {
            if (RequiresValue) return ParsingResult.Error("Value not set.");
            if (!result.IsError)
                argumentInfo.SetValue(target, result.ResultObject);
            return result;
        }
    }
}