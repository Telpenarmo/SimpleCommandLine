using System;
using System.Globalization;

namespace SimpleCommandLine.Parsing.Converters
{
    public static class NumericalValueConverters
    {
        private delegate bool NumberConverter<T>(string s, NumberStyles styles, IFormatProvider provider, out T result);

        private static IValueConverter CreateNumberConverter<T>(NumberConverter<T> converter,
            Func<string, string> errorSelector) => new DelegatingConverter<T>((str, format)
                => converter(str, NumberStyles.Any,
                format, out T result) ? (true, result) : default, errorSelector);

        private static IValueConverter IntegerConverter<T>(NumberConverter<T> converter)
            => CreateNumberConverter(converter,
            (value) => $"\"{value}\" is not a valid integer number.");

        public static IValueConverter Int16Converter
            => IntegerConverter<short>(short.TryParse);
        public static IValueConverter Int32Converter
            => IntegerConverter<int>(int.TryParse);
        public static IValueConverter Int64Converter
            => IntegerConverter<long>(long.TryParse);
        public static IValueConverter ByteConverter
            => IntegerConverter<byte>(byte.TryParse);
        public static IValueConverter SByteConverter
            => IntegerConverter<sbyte>(sbyte.TryParse);
        public static IValueConverter UInt16Converter
            => IntegerConverter<ushort>(ushort.TryParse);
        public static IValueConverter UInt32Converter
            => IntegerConverter<uint>(uint.TryParse);
        public static IValueConverter UInt64Converter
            => IntegerConverter<ulong>(ulong.TryParse);

        private static IValueConverter FloatingPointConverter<T>(NumberConverter<T> converter)
            => CreateNumberConverter(converter, (value) => $"\"{value}\" is not a valid number.");

        public static IValueConverter DoubleConverter
            => FloatingPointConverter<double>(double.TryParse);
        public static IValueConverter FloatConverter
            => FloatingPointConverter<float>(float.TryParse);
        public static IValueConverter DecimalConverter
            => FloatingPointConverter<decimal>(decimal.TryParse);
    }
}