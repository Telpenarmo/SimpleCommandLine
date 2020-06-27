using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class CollectionParser : IArgumentParser
    {
        protected readonly ArgumentInfo argumentInfo;
        protected readonly IMultipleValueConverter collectionConverter;
        private readonly IFormatProvider formatProvider;
        protected readonly List<ValueToken> values = new List<ValueToken>();

        public CollectionParser(ArgumentInfo argumentInfo, IMultipleValueConverter converter, IFormatProvider formatProvider)
        {
            this.argumentInfo = argumentInfo;
            collectionConverter = converter;
            this.formatProvider = formatProvider;
        }

        public bool RequiresValue => values.Count < argumentInfo.Minimum;
        public bool AcceptsValue => values.Count < argumentInfo.Maximum;

        public void AddValue(ValueToken valueToken)
            => values.Add(valueToken);

        public ParsingResult Parse(object target)
        {
            if (RequiresValue) return ParsingResult.Error("Insufficient args given.");

            var result = collectionConverter.Convert(values.Select(v => v.Value).ToArray(), formatProvider);
            if (result.IsError) return result;
            argumentInfo.SetValue(target, result.ResultObject);
            return null;
        }
    }
}