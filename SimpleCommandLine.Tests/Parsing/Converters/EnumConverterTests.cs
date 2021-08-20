using System;
using System.Collections.Generic;
using SimpleCommandLine.Parsing.Converters;
using Xunit;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public class EnumConverterTests
    {
        private static EnumConverter GetInstance(bool ignoreCase, bool acceptNumerical)
            => new(typeof(TestEnum), ignoreCase, acceptNumerical);

        private readonly IFormatProvider culture = System.Globalization.CultureInfo.InvariantCulture;

        [Fact]
        public void With_exact_value_converts_well()
        {
            var conv = GetInstance(false, false);
            var res = conv.Convert(nameof(TestEnum.First), culture);
            Assert.False(res.IsError);
            Assert.Equal(TestEnum.First, res.ResultObject);
        }

        [Theory, MemberData(nameof(Bools))]
        public void With_bad_case_depends_on_setting(bool ignoreCase)
        {
            var conv = GetInstance(ignoreCase, false);
            var res = conv.Convert(nameof(TestEnum.First).ToLowerInvariant(), culture);
            Assert.NotEqual(ignoreCase, res.IsError);
            res = conv.Convert(nameof(TestEnum.First).ToUpperInvariant(), culture);
            Assert.NotEqual(ignoreCase, res.IsError);
        }

        [Theory, MemberData(nameof(Bools))]
        public void With_number_depends_on_setting(bool acceptNumerical)
        {
            var conv = GetInstance(false, acceptNumerical);
            var res = conv.Convert("0", culture);
            Assert.NotEqual(acceptNumerical, res.IsError);
        }

        [Fact]
        public void With_undefined_number_fails()
        {
            var conv = GetInstance(false, true);
            var res = conv.Convert("5", culture);
            Assert.True(res.IsError);
        }

        [Fact]
        public void With_string_representation_of_an_union_converts_to_union()
        {
            var conv = GetInstance(false, false);
            var res = conv.Convert("Second, Third", culture);
            Assert.False(res.IsError);
            Assert.Equal(TestEnum.Second | TestEnum.Third, res.ResultObject);
        }

        public static IEnumerable<object[]> Bools =>
        new[]
        {
            new object[] { true }, new object[] { false },
        };
    }

    enum TestEnum
    {
        First,
        Second = 1,
        Third = 2,
        Union = 3,
        Four = 4
    }
}