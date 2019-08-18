using System;
using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public class DoubleValueConverterTests
    {
        private readonly IValueConverter<double> obj = NumericValueConverters.DoubleConverter;
        private readonly IFormatProvider culture = System.Globalization.CultureInfo.InvariantCulture;

        [Theory, MemberData(nameof(AllowedNumbersStrings))]
        public void WithValidNumberReturnThisNumber(string str, double expected)
        {
            double result = obj.Convert(str, culture);
            Assert.Equal(expected, result);
        }

        [Theory, MemberData(nameof(NullAndEmptyStrings))]
        public void WithNullOrEmptyStringThrows(string str)
        {
            Assert.Throws<ArgumentNullException>(() => obj.Convert(str, culture));
        }

        [Fact]
        public void WithNonNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert("string", culture));
        }

        public static IEnumerable<object[]> AllowedNumbersStrings => TestData.GetAllowedNumbersStrings(true);
        public static IEnumerable<object[]> NullAndEmptyStrings => TestData.GetNullAndEmptyStrings();
    }
}
