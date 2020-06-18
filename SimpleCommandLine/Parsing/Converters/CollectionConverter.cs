using SimpleCommandLine.Parsing;
using System;
using System.Collections.Generic;

namespace SimpleCommandLine
{
    internal abstract class CollectionConverter : IConverter
    {
        protected Type elementType;
        protected IValueConverter valueConverter;

        protected CollectionConverter(Type elementType, IValueConverter valueConverter)
        {
            this.elementType = elementType;
            this.valueConverter = valueConverter;
        }

        public abstract ParsingResult Convert(IReadOnlyList<string> values, IFormatProvider formatProvider);
    }
}