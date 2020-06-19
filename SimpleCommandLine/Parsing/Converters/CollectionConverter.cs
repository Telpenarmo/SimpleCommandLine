using SimpleCommandLine.Parsing;
using System;
using System.Collections.Generic;

namespace SimpleCommandLine
{
    internal abstract class CollectionConverter : IConverter
    {
        protected Type elementType;
        protected IValueConverter elementConverter;

        protected CollectionConverter(Type elementType, IValueConverter elementConverter)
        {
            this.elementType = elementType;
            this.elementConverter = elementConverter;
        }

        public abstract ParsingResult Convert(IReadOnlyList<string> values, IFormatProvider formatProvider);
    }
}