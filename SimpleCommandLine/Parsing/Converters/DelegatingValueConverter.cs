using System;

namespace SimpleCommandLine.Parsing.Converters
{
    public class DelegatingValueConverter<T> : IValueConverter<T>
    {
        private readonly Func<string, IFormatProvider, T> converter;

        public DelegatingValueConverter(Func<string, IFormatProvider, T> converter)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
        }

        public DelegatingValueConverter(Func<string, IFormatProvider, (bool, T)> converter,
            Func<string, FormatException> errorSelector)
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
                return isSuccess ? result : throw errorSelector(value);
            };
        }

        public T Convert(string value, IFormatProvider formatProvider) => converter(value, formatProvider);

        object IValueConverter.Convert(string value, IFormatProvider formatProvider) => Convert(value, formatProvider);
    }
}
