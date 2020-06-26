using System;
using System.Globalization;
using System.Net;

namespace SimpleCommandLine.Parsing.Converters
{
    internal static class StockConverters
    {
        public static IValueConverter StringConverter
            => new DelegatingConverter<string>((value, format) => ParsingResult.Success(value));

        public static IValueConverter GuidConverter
            => new DelegatingConverter<Guid>(
                (value, format) => Guid.TryParse(value, out var result) ? (true, result) : default,
                value => $"\"{value}\" is not a valid GUID.");

        public static IValueConverter IPAdressConverter
            => new DelegatingConverter<IPAddress>(
                (value, format) => IPAddress.TryParse(value, out var result) ? (true, result) : default,
                value => $"\"{value}\" is not a valid IP address.");

        public static IValueConverter URIConverter
            => new DelegatingConverter<Uri>((value, format) => ParsingResult.Success(new Uri(value)));

        private delegate bool TimeConverter<T>(string value, IFormatProvider format, DateTimeStyles styles, out T result);
        private static IValueConverter CreateDateTimeConverter<T>(TimeConverter<T> converter, Func<string, string> errorSelector)
            => new DelegatingConverter<T>((str, format)
                => converter(str, format,
                DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite
                    | DateTimeStyles.AllowWhiteSpaces,
                out T result) ? (true, result) : default, errorSelector);

        public static IValueConverter DateTimeConverter
            => CreateDateTimeConverter<DateTime>(DateTime.TryParse,
            value => $"\"{value}\" is not a valid date nor time.");
        public static IValueConverter DateTimeOffsetConverter
            => CreateDateTimeConverter<DateTimeOffset>(DateTimeOffset.TryParse,
            value => $"\"{value}\" is not a valid date ot time offset.");
        public static IValueConverter TimeSpanConverter
            => new DelegatingConverter<TimeSpan>(
                (value, format) => TimeSpan.TryParse(value, format, out var result) ? (true, result) : default,
                value => $"\"{value}\" is not a valid time span value.");
    }
}