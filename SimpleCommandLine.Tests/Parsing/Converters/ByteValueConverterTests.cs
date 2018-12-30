using System;
using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public class ByteValueConverterTests
    {
        private readonly ByteValueConverter obj = new ByteValueConverter();
        private readonly IFormatProvider culture = System.Globalization.CultureInfo.InvariantCulture;

        [Fact]
        public void WithValidNumberReturnThisNumber()
        {
            byte result = obj.Convert(" 1 ", culture);
            Assert.Equal(1, result);
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

        [Fact]
        public void WithNegativeNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert("-1", culture));
        }

        public static IEnumerable<object[]> NullAndEmptyStrings => TestData.GetNullAndEmptyStrings();
    }
}
