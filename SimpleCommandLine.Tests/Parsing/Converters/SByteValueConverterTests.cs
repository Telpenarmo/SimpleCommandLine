using System;
using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public class SByteValueConverterTests
    {
        private readonly SByteValueConverter obj = new SByteValueConverter();
        private readonly IFormatProvider culture = System.Globalization.CultureInfo.InvariantCulture;

        [Theory, InlineData(" 1 ", 1), InlineData("-1", -1)]
        public void WithValidNumberReturnThisNumber(string s, sbyte expected)
        {
            sbyte result = obj.Convert(s, culture);
            Assert.Equal(expected, result);
        }

        [Theory, MemberData(nameof(NullAndEmptyStrings))]
        public void WithNullOrEmptyStringThrows(string str)
        {
            Assert.Throws<FormatException>(() => obj.Convert(str, culture));
        }

        [Fact]
        public void WithDoubleNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert("1.1", culture));
        }

        [Fact]
        public void WithIntNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert(int.MaxValue.ToString(), culture));
        }

        [Fact]
        public void WithNonNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert("string", culture));
        }

        public static IEnumerable<object[]> NullAndEmptyStrings => TestData.GetNullAndEmptyStrings();
    }
}
