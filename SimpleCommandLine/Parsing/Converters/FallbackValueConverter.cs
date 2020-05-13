using System;
using System.ComponentModel;
using System.Reflection;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class FallbackValueConverter : IValueConverter
    {
        readonly TypeConverter converter;
        readonly ReflectionConverter reflectionConverter;

        public bool CanConvert => converter?.CanConvertFrom(typeof(string)) | reflectionConverter?.CanConvert ?? false;

        public FallbackValueConverter(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(converter));

            converter = TypeDescriptor.GetConverter(type);

            if (!CanConvert)
            {
                converter = null;
                reflectionConverter = new ReflectionConverter(type);
            }
        }

        public object Convert(string str, IFormatProvider formatProvider)
        {
            if (CanConvert)
                return converter?.ConvertFromString(str)
                    ?? reflectionConverter.Convert(str, formatProvider);
            else throw new InvalidOperationException();
        }

        private class ReflectionConverter : IValueConverter
        {
            readonly ConstructorInfo stringConstructor;
            readonly ConstructorInfo stringAndFormatConstructor;
            readonly ConstructorInfo formatAndStringConstructor;

            public bool CanConvert =>
                stringAndFormatConstructor != null ||
                stringAndFormatConstructor != null ||
                stringConstructor != null;

            public ReflectionConverter(Type type)
            {
                stringConstructor = type.GetConstructor(new Type[] { typeof(string) });
                stringAndFormatConstructor = type.GetConstructor(new Type[] { typeof(string), typeof(IFormatProvider) });
                formatAndStringConstructor = type.GetConstructor(new Type[] { typeof(IFormatProvider), typeof(string) });
            }

            public object Convert(string str, IFormatProvider formatProvider) =>
                stringAndFormatConstructor?.Invoke(new object[] { str, formatProvider })
                    ?? formatAndStringConstructor?.Invoke(new object[] { formatProvider, str })
                    ?? stringConstructor?.Invoke(new object[] { str });
        }
    }
}