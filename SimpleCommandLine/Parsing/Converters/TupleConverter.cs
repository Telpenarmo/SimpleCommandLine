using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class TupleConverter : IMultipleValueConverter
    {
        private readonly Type type;
        private readonly int valuesNumber;

        public TupleConverter(Type type, int valuesNumber, IEnumerable<IConverter> elementConverters)
        {
            this.type = type;
            this.valuesNumber = valuesNumber;
            ElementConverters = elementConverters;
        }

        public IEnumerable<IConverter> ElementConverters { get; }

        public ParsingResult Convert(IReadOnlyList<object> values)
        {
            if (values.Count != valuesNumber)
                return ParsingResult.Error($"{values.Count} values given, while exactly {valuesNumber} expected.");
            return ParsingResult.Success(Activator.CreateInstance(type, values));
        }
    }
}