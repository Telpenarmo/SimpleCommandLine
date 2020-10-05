using System;
using System.ComponentModel;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class TypeDescriptorConverter : ISingleValueConverter
    {
        private readonly TypeConverter wrapped;

        public TypeDescriptorConverter(TypeConverter wrapped)
        {
            this.wrapped = wrapped;
        }

        public ParsingResult Convert(string value, IFormatProvider formatProvider)
        {
            return ParsingResult.Success(wrapped.ConvertFromString(value));
        }
    }
}