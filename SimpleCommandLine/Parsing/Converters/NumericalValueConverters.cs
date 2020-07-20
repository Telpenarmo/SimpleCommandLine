using System;
using System.Globalization;

namespace SimpleCommandLine.Parsing.Converters
{
    public static class NumericalValueConverters
    {
        private delegate bool NumberConverter<T>(string s, NumberStyles styles, IFormatProvider provider, out T result);

        private static ISingleValueConverter CreateNumberConverter<T>(NumberConverter<T> converter,
            Func<string, string> errorSelector)
            => new DelegatingConverter<T>(
                (str, format) => converter(str, NumberStyles.Any, format, out T result)
                    ? (true, result) : default, errorSelector);

        private static ISingleValueConverter IntegerConverter<T>(NumberConverter<T> converter)
            => CreateNumberConverter(converter,
            (value) => $"\"{value}\" is not a valid integer number.");

        public static ISingleValueConverter Int16Converter
            => IntegerConverter<short>(short.TryParse);
        public static ISingleValueConverter Int32Converter
            => IntegerConverter<int>(int.TryParse);
        public static ISingleValueConverter Int64Converter
            => IntegerConverter<long>(long.TryParse);
        public static ISingleValueConverter ByteConverter
            => IntegerConverter<byte>(byte.TryParse);
        public static ISingleValueConverter SByteConverter
            => IntegerConverter<sbyte>(sbyte.TryParse);
        public static ISingleValueConverter UInt16Converter
            => IntegerConverter<ushort>(ushort.TryParse);
        public static ISingleValueConverter UInt32Converter
            => IntegerConverter<uint>(uint.TryParse);
        public static ISingleValueConverter UInt64Converter
            => IntegerConverter<ulong>(ulong.TryParse);

        private static ISingleValueConverter FloatingPointConverter<T>(NumberConverter<T> converter)
            => CreateNumberConverter(converter, (value) => $"\"{value}\" is not a valid number.");

        public static ISingleValueConverter DoubleConverter
            => FloatingPointConverter<double>(double.TryParse);
        public static ISingleValueConverter FloatConverter
            => FloatingPointConverter<float>(float.TryParse);
        public static ISingleValueConverter DecimalConverter
            => FloatingPointConverter<decimal>(decimal.TryParse);
    }
}