using System;
using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public class UShortValueConverterTests
    {
        private readonly IValueConverter<ushort> obj = NumericValueConverters.UInt16Converter;
        private readonly IFormatProvider culture = System.Globalization.CultureInfo.InvariantCulture;

        [Theory, MemberData(nameof(AllowedNumbersStrings))]
        public void WithValidNumberReturnThisNumber(string str, ushort expected)
        {
            ushort result = obj.Convert(str, culture);
            Assert.Equal(expected, result);
        }

        [Theory, MemberData(nameof(NullAndEmptyStrings))]
        public void WithNullOrEmptyStringThrows(string str)
        {
            Assert.Throws<ArgumentNullException>(() => obj.Convert(str, culture));
        }

        [Fact]
        public void WithDoubleNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert("1.1", culture));
        }

        [Fact]
        public void WithLongNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert(int.MaxValue.ToString(), culture));
        }

        [Fact]
        public void WithNonNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert("string", culture));
        }

        [Fact]
        public void WithNegativeNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert("-1", culture));
        }

        public static IEnumerable<object[]> AllowedNumbersStrings => TestData.GetAllowedNumbersStrings(false, false);
        public static IEnumerable<object[]> NullAndEmptyStrings => TestData.GetNullAndEmptyStrings();
    }
}
