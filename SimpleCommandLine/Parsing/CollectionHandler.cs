using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class CollectionHandler : IArgumentHandler
    {
        public ParameterInfo ParameterInfo { get; }
        protected readonly IConverter converter;
        private readonly IFormatProvider formatProvider;
        protected readonly List<ValueToken> tokens = new();
        protected ValuesGroupToken? group;

        public CollectionHandler(ParameterInfo parameterInfo, IConverter converter, IFormatProvider formatProvider)
        {
            ParameterInfo = parameterInfo;
            this.converter = converter;
            this.formatProvider = formatProvider;
        }

        public bool RequiresValue => group is null && tokens.Count < ParameterInfo.Minimum;
        public bool AcceptsValue => group is null && tokens.Count < ParameterInfo.Maximum;

        public void AddValue(ValueToken token)
        {
            if (group != null) throw new InvalidOperationException("Value already set.");
            if (!AcceptsValue) throw new InvalidOperationException("Maximum values number reached.");
            tokens.Add(token);
        }

        public void SetValue(ValueToken token)
        {
            if (tokens.Any() || group != null) throw new InvalidOperationException("Value already set.");
            group = token is ValuesGroupToken g ? g
                : throw new InvalidOperationException("Values group expected.");
        }

        public ParsingResult GetResult()
        {
            if (RequiresValue) return ParsingResult.Error("Insufficient args given.");
            group ??= new ValuesGroupToken(tokens, string.Empty);

            return ParseRecursively(group, converter);
        }

        private ParsingResult ParseRecursively(ValueToken token, IConverter converter)
        {
            if (converter is ISingleValueConverter singleConverter)
                return singleConverter.Convert(token.Value, formatProvider);
            if (converter is IMultipleValueConverter multipleConverter)
            {
                if (token is ValuesGroupToken g)
                {
                    using var enumerator = multipleConverter.ElementConverters.GetEnumerator();
                    var values = new object[g.Tokens.Count];
                    for (int i = 0; i < g.Tokens.Count && enumerator.MoveNext(); i++)
                    {
                        var result = ParseRecursively(g.Tokens[i], enumerator.Current);
                        if (result.ResultObject != null) values[i] = result.ResultObject;
                        else return result;
                    }
                    return multipleConverter.Convert(values);
                }
                var res = ParseRecursively(token, multipleConverter.ElementConverters.First());
                if (res.ResultObject != null)
                    return multipleConverter.Convert(new[] { res.ResultObject });
                return res;
            }
            return ParsingResult.Error("Invalid values grouping.");
        }
    }
}