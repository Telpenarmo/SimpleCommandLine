using System;
using System.Globalization;
using System.Net;

namespace SimpleCommandLine.Parsing.Converters
{
    internal static class StockConverters
    {
        public static ISingleValueConverter StringConverter
            => new DelegatingConverter<string>((value, format) => ParsingResult.Success(value));

        public static ISingleValueConverter GuidConverter
            => new DelegatingConverter<Guid>(
                (value, format) => Guid.TryParse(value, out var result) ? (true, result) : default,
                value => $"\"{value}\" is not a valid GUID.");

        public static ISingleValueConverter IPAdressConverter
            => new DelegatingConverter<IPAddress>(
                (value, format) => IPAddress.TryParse(value, out var result) ? (true, result) : default,
                value => $"\"{value}\" is not a valid IP address.");

        public static ISingleValueConverter URIConverter
            => new DelegatingConverter<Uri>((value, format) => ParsingResult.Success(new Uri(value)));

        private delegate bool TimeConverter<T>(string value, IFormatProvider format, DateTimeStyles styles, out T result);
        private static ISingleValueConverter CreateDateTimeConverter<T>(TimeConverter<T> converter, Func<string, string> errorSelector)
            => new DelegatingConverter<T>((str, format)
                => converter(str, format,
                DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite
                    | DateTimeStyles.AllowWhiteSpaces,
                out T result) ? (true, result) : default, errorSelector);

        public static ISingleValueConverter DateTimeConverter
            => CreateDateTimeConverter<DateTime>(DateTime.TryParse,
            value => $"\"{value}\" is not a valid date nor time.");
        public static ISingleValueConverter DateTimeOffsetConverter
            => CreateDateTimeConverter<DateTimeOffset>(DateTimeOffset.TryParse,
            value => $"\"{value}\" is not a valid date ot time offset.");
        public static ISingleValueConverter TimeSpanConverter
            => new DelegatingConverter<TimeSpan>(
                (value, format) => TimeSpan.TryParse(value, format, out var result) ? (true, result) : default,
                value => $"\"{value}\" is not a valid time span value.");
    }
}