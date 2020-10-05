using System;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class UnaryArgumentHandler : IArgumentHandler
    {
        public ParameterInfo ParameterInfo { get; }
        protected readonly ISingleValueConverter valueConverter;
        protected readonly IFormatProvider formatProvider;
        protected ParsingResult? result;

        public UnaryArgumentHandler(ParameterInfo parameterInfo, IConverter converter, IFormatProvider formatProvider)
        {
            ParameterInfo = parameterInfo;
            valueConverter = converter as ISingleValueConverter
                ?? throw new Exception("Wrong converter given.");
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

        public virtual ParsingResult GetResult()
            => result is null ? ParsingResult.Error("Value not set.") : result;
    }
}