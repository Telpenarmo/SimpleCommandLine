using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class TupleConverter : IMultipleValueConverter
    {
        private readonly ISingleValueConverter[] converters;
        private readonly Type type;
        private readonly int valuesNumber;

        public TupleConverter(ISingleValueConverter[] converters, Type type, int valuesNumber)
        {
            if (converters.Length != valuesNumber)
                throw new InvalidOperationException(
                    "Number of converters must be equal to the number of values.");
            this.converters = converters;
            this.type = type;
            this.valuesNumber = valuesNumber;
        }

        public ParsingResult Convert(IReadOnlyList<string> values, IFormatProvider formatProvider)
        {
            if (values.Count != valuesNumber)
                return ParsingResult.Error("The number of params is wrong."); // TODO: dokładniejszy komentarz
            ParsingResult[] results = new ParsingResult[valuesNumber];
            for (int i = 0; i < valuesNumber; i++)
            {
                results[i] = converters[i].Convert(values[i], formatProvider);
                if (results[i].IsError) return results[i];
            }
            return ParsingResult.Success(Activator.CreateInstance(type, results));
        }
    }
}