using System;
using System.Linq;

namespace SimpleCommandLine.Parsing.Converters
{
    class BoolValueConverter : IValueConverter<bool>
    {
        readonly string[] trueAliases;
        readonly string[] falseAliases;

        public BoolValueConverter()
        {
            trueAliases = new[] { "true", "t", "on", "1", "yes", "y", "+" };
            falseAliases = new[] { "false", "f", "off", "0", "no", "n", "-" };
        }

        public BoolValueConverter(string[] trueAliases, string[] falseAliases)
        {
            this.trueAliases = trueAliases ?? throw new ArgumentNullException(nameof(trueAliases));
            this.falseAliases = falseAliases ?? throw new ArgumentNullException(nameof(falseAliases));
        }

        public bool Convert(string str, IFormatProvider formatProvider)
        {
            if (trueAliases.Contains(str, StringComparer.OrdinalIgnoreCase)) return true;
            else if (falseAliases.Contains(str, StringComparer.OrdinalIgnoreCase)) return false;
            else if (bool.TryParse(str, out bool result))
                return result;
            else throw new FormatException("Value is not valid.");
        }

        object IValueConverter.Convert(string str, IFormatProvider formatProvider) => Convert(str, formatProvider);
    }
}
