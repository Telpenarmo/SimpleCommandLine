﻿using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Parsing
{
    internal class CollectionParser : IArgumentParser
    {
        protected readonly ArgumentInfo argumentInfo;
        protected readonly IMultipleValueConverter converter;
        private readonly IFormatProvider formatProvider;
        protected readonly List<ValueToken> tokens = new List<ValueToken>();
        protected ValuesGroupToken group;

        public CollectionParser(ArgumentInfo argumentInfo, IMultipleValueConverter converter, IFormatProvider formatProvider)
        {
            this.argumentInfo = argumentInfo;
            this.converter = converter;
            this.formatProvider = formatProvider;
        }

        public bool RequiresValue => group is null && tokens.Count < argumentInfo.Minimum;
        public bool AcceptsValue => group is null && tokens.Count < argumentInfo.Maximum;

        public void AddValue(ValueToken token)
        {
            if (group != null) throw new InvalidOperationException("Value already set.");
            if (!AcceptsValue) throw new InvalidOperationException("Maximum values number reached.");
            tokens.Add(token);
        }

        public void SetValue(ValueToken token)
        {
            if (tokens.Any()) throw new InvalidOperationException("Value already set.");
            group = token is ValuesGroupToken g ? g
                : throw new InvalidOperationException("Values group expected.");
        }

        public ParsingResult Parse(object target)
        {
            if (RequiresValue) return ParsingResult.Error("Insufficient args given.");
            group ??= new ValuesGroupToken(tokens, string.Empty);

            var result = ParseRecursively(group, converter);
            if (result.IsError) return result;
            argumentInfo.SetValue(target, result.ResultObject);
            return null;
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
                        if (result.IsError) return result;
                        else values[i] = result.ResultObject;
                    }
                    return multipleConverter.Convert(values);
                }
                var res = ParseRecursively(token, multipleConverter.ElementConverters.First());
                if (res.IsError) return res;
                return multipleConverter.Convert(new[] { res.ResultObject });
            }
            return ParsingResult.Error("Invalid values grouping.");
        }
    }
}