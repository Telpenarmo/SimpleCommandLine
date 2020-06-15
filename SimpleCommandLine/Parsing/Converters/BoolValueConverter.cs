using System;
using System.Linq;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class BoolValueConverter : IValueConverter
    {
        readonly string[] trueAliases;
        readonly string[] falseAliases;

        public BoolValueConverter()
        {
            trueAliases = new[] { string.Empty, "true", "t", "on", "1", "yes", "y", "+" };
            falseAliases = new[] { "false", "f", "off", "0", "no", "n", "-" };
        }

        public BoolValueConverter(string[] trueAliases, string[] falseAliases)
        {
            this.trueAliases = trueAliases ?? throw new ArgumentNullException(nameof(trueAliases));
            this.falseAliases = falseAliases ?? throw new ArgumentNullException(nameof(falseAliases));
        }

        public bool Convert(string str, IFormatProvider formatProvider, out object result)
        {
            if (trueAliases.Contains(str, StringComparer.OrdinalIgnoreCase))
                result = true;
            else if (falseAliases.Contains(str, StringComparer.OrdinalIgnoreCase))
                result = false;
            else if (bool.TryParse(str, out bool b))
                result = b;
            else
                result = null;
            return result != null;
        }
    }
}