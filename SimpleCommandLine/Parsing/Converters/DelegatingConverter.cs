using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class DelegatingConverter<T> : IValueConverter
    {
        private readonly Func<string, IFormatProvider, ParsingResult> converter;

        public DelegatingConverter(Func<string, IFormatProvider, ParsingResult> converter)
            => this.converter = converter ?? throw new ArgumentNullException(nameof(converter));

        public DelegatingConverter(Func<string, IFormatProvider, (bool, T)> converter,
            Func<string, string> errorSelector)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            if (errorSelector == null)
                throw new ArgumentNullException(nameof(errorSelector));

            this.converter = (value, format) =>
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("The value must not be empty.");
                var (isSuccess, result) = converter(value, format);
                return isSuccess ? ParsingResult.Success(result)
                : ParsingResult.Error(errorSelector(value)); 
            };
        }

        public ParsingResult Convert(string value, IFormatProvider formatProvider)
            => converter(value, formatProvider);
    }
}