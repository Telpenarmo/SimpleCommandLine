using System;
using System.Globalization;
using System.Net;

namespace SimpleCommandLine.Parsing.Converters
{
    internal static class StockConverters
    {
        public static IValueConverter<string> StringConverter
            => new DelegatingValueConverter<string>((value, format) => value);

        public static IValueConverter<Guid> GuidConverter
            => new DelegatingValueConverter<Guid>((value, format) => Guid.TryParse(value, out var result) ? (true, result) : default,
                value => new FormatException($"\"{value}\" is not a valid GUID."));

        public static IValueConverter<IPAddress> IPAdressConverter
            => new DelegatingValueConverter<IPAddress>((value, format) => IPAddress.TryParse(value, out var result) ? (true, result) : default,
                value => new FormatException($"\"{value}\" is not a valid IP address."));

        public static IValueConverter<Uri> URIConverter
            => new DelegatingValueConverter<Uri>((value, format) => new Uri(value));

        private delegate bool TimeConverter<T>(string value, IFormatProvider format, DateTimeStyles styles, out T result);
        private static IValueConverter<T> CreateDateTimeConverter<T>(TimeConverter<T> converter, Func<string, FormatException> errorSelector) =>
            new DelegatingValueConverter<T>((str, format)
                => converter(str, format,
                DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces,
                out T result) ? (true, result) : default, errorSelector);

        public static IValueConverter<DateTime> DateTimeConverter
            => CreateDateTimeConverter<DateTime>(DateTime.TryParse, value => throw new FormatException($"\"{value}\" is not a valid date nor time."));
        public static IValueConverter<DateTimeOffset> DateTimeOffsetConverter
            => CreateDateTimeConverter<DateTimeOffset>(DateTimeOffset.TryParse, value => throw new FormatException($"\"{value}\" is not a valid date ot time offset."));
        public static IValueConverter<TimeSpan> TimeSpanConverter
            => new DelegatingValueConverter<TimeSpan>((value, format) => TimeSpan.TryParse(value, format, out var result) ? (true, result) : default,
                value => new FormatException($"\"{value}\" is not a valid time span value."));
    }
}
