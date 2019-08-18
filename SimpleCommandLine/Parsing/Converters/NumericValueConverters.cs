using System;
using System.Globalization;

namespace SimpleCommandLine.Parsing.Converters
{
    public static class NumericValueConverters
    {
        private delegate bool NumberConverter<T>(string s, NumberStyles styles, IFormatProvider provider, out T result);

        private static IValueConverter<T> CreateNumberConverter<T>(NumberConverter<T> converter, Func<string, FormatException> errorSelector)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            if (errorSelector == null)
                throw new ArgumentNullException(nameof(errorSelector));

            return new DelegatingValueConverter<T>((str, format)
                => converter(str, NumberStyles.Any,
                format, out T result) ? (true, result) : default, errorSelector);
        }

        private static IValueConverter<T> IntegerConverter<T>(NumberConverter<T> converter)
            => CreateNumberConverter(converter, (value) => throw new FormatException($"\"{value}\" is not a valid integer number."));

        public static IValueConverter<short> Int16Converter
            => IntegerConverter<short>(short.TryParse);
        public static IValueConverter<int> Int32Converter
            => IntegerConverter<int>(int.TryParse);
        public static IValueConverter<long> Int64Converter
            => IntegerConverter<long>(long.TryParse);
        public static IValueConverter<byte> ByteConverter
            => IntegerConverter<byte>(byte.TryParse);
        public static IValueConverter<sbyte> SByteConverter
            => IntegerConverter<sbyte>(sbyte.TryParse);
        public static IValueConverter<ushort> UInt16Converter
            => IntegerConverter<ushort>(ushort.TryParse);
        public static IValueConverter<uint> UInt32Converter
            => IntegerConverter<uint>(uint.TryParse);
        public static IValueConverter<ulong> UInt64Converter
            => IntegerConverter<ulong>(ulong.TryParse);

        private static IValueConverter<T> FloatingPointConverter<T>(NumberConverter<T> converter)
            => CreateNumberConverter(converter, (value) => throw new FormatException($"\"{value}\" is not a valid number."));

        public static IValueConverter<double> DoubleConverter
            => FloatingPointConverter<double>(double.TryParse);
        public static IValueConverter<float> FloatConverter
            => FloatingPointConverter<float>(float.TryParse);
        public static IValueConverter<decimal> DecimalConverter
            => FloatingPointConverter<decimal>(decimal.TryParse);
    }
}