using System;
using System.Linq;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class BoolValueConverter : ISingleValueConverter
    {
        private readonly string[] trueAliases;
        private readonly string[] falseAliases;

        public BoolValueConverter(string[] trueAliases, string[] falseAliases)
        {
            this.trueAliases = trueAliases;
            this.falseAliases = falseAliases;
        }

        public ParsingResult Convert(string str, IFormatProvider formatProvider)
        {
            ParsingResult result;
            if (trueAliases.Contains(str, StringComparer.OrdinalIgnoreCase))
                result = ParsingResult.Success(true);
            else if (falseAliases.Contains(str, StringComparer.OrdinalIgnoreCase))
                result = ParsingResult.Success(false);
            else
                result = ParsingResult.Error($"\"{str}\" is not a valid boolean.");
            return result;
        }

        public object DefaultValue => true;
    }
}