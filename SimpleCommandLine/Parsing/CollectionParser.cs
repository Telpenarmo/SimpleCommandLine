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
        protected readonly CollectionConverter collectionConverter;
        private readonly IFormatProvider formatProvider;
        protected readonly List<ValueToken> values = new List<ValueToken>();

        public CollectionParser(ArgumentInfo argumentInfo, CollectionConverter converter, IFormatProvider formatProvider)
        {
            this.argumentInfo = argumentInfo;
            collectionConverter = converter;
            this.formatProvider = formatProvider;
        }

        public bool RequiresValue => values.Count < argumentInfo.Minimum;

        public bool AcceptsValue => values.Count < argumentInfo.Maximum;

        public void AddValue(ValueToken valueToken)
            => values.Add(valueToken);

        public void Parse(object target)
        {
            if (!collectionConverter.Convert(values.Select(v => v.Value).ToArray(), formatProvider, out object result))
                throw new ArgumentException();
            argumentInfo.SetValue(target, result);
        }
    }
}