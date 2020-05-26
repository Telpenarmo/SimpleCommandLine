using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class CollectionParser : IArgumentParser
    {
        protected readonly ParsingArgumentInfo argumentInfo;
        protected readonly CollectionConverter collectionConverter;
        protected readonly List<ValueToken> values = new List<ValueToken>();

        public CollectionParser(ParsingArgumentInfo argumentInfo, CollectionConverter converter)
        {
            this.argumentInfo = argumentInfo;
            collectionConverter = converter;
        }

        public bool RequiresValue => values.Count < argumentInfo.Minimum;

        public bool AcceptsValue => values.Count < argumentInfo.Maximum;

        public void AddValue(ValueToken valueToken)
            => values.Add(valueToken);

        public void Parse(object target, IFormatProvider formatProvider)
        {
            argumentInfo.SetValue(target, collectionConverter.Convert(values.Select(v => v.Value).ToArray(), formatProvider));
        }
    }
}
