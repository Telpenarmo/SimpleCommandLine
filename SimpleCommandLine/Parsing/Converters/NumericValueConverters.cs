using System;
using System.Globalization;

namespace SimpleCommandLine.Parsing.Converters
{
    public static class NumericValueConverters
    {
        private delegate bool NumberConverter<T>(string s, NumberStyles styles, IFormatProvider provider, out T result);

        private static IValueConverter<T> CreateNumberConverter<T>(NumberConverter<T> converter, NumberStyles styles, Func<string, FormatException> errorSelector)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            if (errorSelector == null)
                throw new ArgumentNullException(nameof(errorSelector));

            return new DelegatingValueConverter<T>((str, format)
                => converter(str,
                styles | NumberStyles.AllowCurrencySymbol | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite,
                format, out T result) ? (true, result) : default, errorSelector);
        }

        private static IValueConverter<T> IntegerConverter<T>(NumberConverter<T> converter, NumberStyles styles)
            => CreateNumberConverter(converter, styles, (value) => throw new FormatException($"\"{value}\" is not a valid integer number."));

        public static IValueConverter<short> Int16Converter
            => IntegerConverter<short>(short.TryParse, Signed(NumberStyles.None));
        public static IValueConverter<int> Int32Converter
            => IntegerConverter<int>(int.TryParse, Signed(LongNumberStyle));
        public static IValueConverter<long> Int64Converter
            => IntegerConverter<long>(long.TryParse, Signed(LongNumberStyle));
        public static IValueConverter<byte> ByteConverter
            => IntegerConverter<byte>(byte.TryParse, NumberStyles.None);
        public static IValueConverter<sbyte> SByteConverter
            => IntegerConverter<sbyte>(sbyte.TryParse, Signed(NumberStyles.None));
        public static IValueConverter<ushort> UInt16Converter
            => IntegerConverter<ushort>(ushort.TryParse, NumberStyles.None);
        public static IValueConverter<uint> UInt32Converter
            => IntegerConverter<uint>(uint.TryParse, LongNumberStyle);
        public static IValueConverter<ulong> UInt64Converter
            => IntegerConverter<ulong>(ulong.TryParse, LongNumberStyle);

        private static IValueConverter<T> FloatingPointConverter<T>(NumberConverter<T> converter, NumberStyles styles)
            => CreateNumberConverter(converter, styles | NumberStyles.Number, (value) => throw new FormatException($"\"{value}\" is not a valid number."));

        public static IValueConverter<double> DoubleConverter
            => FloatingPointConverter<double>(double.TryParse, Signed(NumberStyles.None));
        public static IValueConverter<float> FloatConverter
            => FloatingPointConverter<float>(float.TryParse, Signed(NumberStyles.None));
        public static IValueConverter<decimal> DecimalConverter
            => FloatingPointConverter<decimal>(decimal.TryParse, Signed(NumberStyles.None));

        private static NumberStyles LongNumberStyle => NumberStyles.AllowExponent | NumberStyles.AllowThousands;
        private static NumberStyles Signed(NumberStyles styles) => styles | NumberStyles.AllowLeadingSign | NumberStyles.AllowTrailingSign;
    }
}