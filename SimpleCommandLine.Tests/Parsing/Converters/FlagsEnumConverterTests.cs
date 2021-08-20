using System;
using System.Collections.Generic;
using SimpleCommandLine.Parsing.Converters;
using Xunit;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public class FlagsEnumConverterTests
    {
        private static FlagsEnumConverter GetInstance(bool ignoreCase, bool acceptNumerical)
            => new(typeof(TestEnum), ignoreCase, acceptNumerical);

        [Fact]
        public void With_two_valid_enums_converts_to_union()
        {
            var conv = GetInstance(false, false);
            var input = new dynamic[] { TestEnum.Second, TestEnum.Third };
            var res = conv.Convert(input);
            Assert.False(res.IsError);
            Assert.Equal(TestEnum.Second | TestEnum.Third, res.ResultObject);
        }

        [Fact]
        public void With_two_positive_integers_converts_to_union()
        {
            var conv = GetInstance(false, false);
            var input = new dynamic[] { 1, 4 };
            var res = conv.Convert(input);
            Assert.False(res.IsError);
            Assert.Equal(TestEnum.Second | TestEnum.Four, res.ResultObject);
        }

        [Fact]
        public void With_negative_integer_throws()
        {
            var conv = GetInstance(false, false);
            var input = new dynamic[] { 1, -2 };
            Assert.Throws<ArgumentException>(() => conv.Convert(input));
        }

        [Fact]
        public void With_non_integer_nor_valid_enum_throws()
        {
            var conv = GetInstance(false, false);
            var input = new dynamic[] { "string" };
            Assert.Throws<ArgumentException>(() => conv.Convert(input));
        }
    }
}