using System;
using System.Linq;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class BoolValueConverter : ISingleValueConverter
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

        public ParsingResult Convert(string str, IFormatProvider formatProvider)
        {
            ParsingResult result;
            if (trueAliases.Contains(str, StringComparer.OrdinalIgnoreCase))
                result = ParsingResult.Success(true);
            else if (falseAliases.Contains(str, StringComparer.OrdinalIgnoreCase))
                result = ParsingResult.Success(false);
            else if (bool.TryParse(str, out bool b))
                result = ParsingResult.Success(b);
            else
                result = ParsingResult.Error($"\"{str}\" is not a valid boolean.");
            return result;
        }

        public ParsingResult DefaultValue => ParsingResult.Success(true);
    }
}