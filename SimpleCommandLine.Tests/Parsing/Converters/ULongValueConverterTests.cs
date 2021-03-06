﻿using System;
using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing.Converters;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public class ULongValueConverterTests
    {
        private readonly IValueConverter<ulong> obj = NumericValueConverters.UInt64Converter;
        private readonly IFormatProvider culture = System.Globalization.CultureInfo.InvariantCulture;

        [Theory, MemberData(nameof(AllowedNumbersStrings))]
        public void WithValidNumberReturnThisNumber(string str, ulong expected)
        {
            ulong result = obj.Convert(str, culture);
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
        public void WithNonNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert("string", culture));
        }

        [Fact]
        public void WithNegativeNumberThrows()
        {
            Assert.Throws<FormatException>(() => obj.Convert("-1", culture));
        }

        public static IEnumerable<object[]> AllowedNumbersStrings => TestData.GetAllowedNumbersStrings(false);
        public static IEnumerable<object[]> NullAndEmptyStrings => TestData.GetNullAndEmptyStrings();
    }
}
