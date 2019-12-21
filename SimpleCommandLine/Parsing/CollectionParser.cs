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
            this.argumentInfo = argumentInfo ?? throw new ArgumentNullException(nameof(argumentInfo));
            collectionConverter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public bool RequiresValue => values.Count < argumentInfo.Minimum;

        public bool AcceptsValue => values.Count < argumentInfo.Maximum;

        public void AddValue(IValueToken valueToken)
        {
            if (valueToken is ValuesGroupToken group)
                values.AddRange(group.Tokens.OfType<ValueToken>());
            else
                values.Add(valueToken as ValueToken);
        }

        public void Parse(object target, IFormatProvider formatProvider)
        {
            if (values.Count > argumentInfo.Maximum)
                throw new ArgumentException("Too many values provided.");
            argumentInfo.SetValue(target, collectionConverter.Convert(values.Select(v => v.Value).ToArray(), formatProvider));
        }
    }
}
