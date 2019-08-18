using System;
using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing.Converters;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public class FallbackValueConverterTests
    {
        private readonly IFormatProvider culture = System.Globalization.CultureInfo.InvariantCulture;

        [Theory, MemberData(nameof(SupportedTypes))]
        internal void With_supported_type_CanConvert_returns_true(Type type)
        {
            var converter = new FallbackValueConverter(type);
            Assert.True(converter.CanConvert);
        }

        [Theory, MemberData(nameof(UnsupportedTypes))]
        internal void With_not_supported_type_CanConvert_returns_false(Type type)
        {
            var converter = new FallbackValueConverter(type);
            Assert.False(converter.CanConvert);
        }

        [Theory, MemberData(nameof(ConvertingData))]
        public void With_supported_type_Convert_returns_value(Type type, object expected, string value)
        {
            var converter = new FallbackValueConverter(type);
            var result = converter.Convert(value, culture);
            Assert.IsType(type, result);
            if (!(expected is A a))
                Assert.Equal(expected, result);
            else
                Assert.True(expected.Equals(a));
        }

        [Theory, MemberData(nameof(UnsupportedTypes))]
        public void With_not_supported_type_Convert_throws(Type type)
        {
            var converter = new FallbackValueConverter(type);
            Assert.Throws<InvalidOperationException>(() => converter.Convert(string.Empty, culture));
        }

        [Fact]
        internal void With_null_constructor_throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FallbackValueConverter(null));
        }

        public static IEnumerable<object[]> SupportedTypes()
        {
            yield return new[] { typeof(Uri) };
            yield return new[] { typeof(A) };
            yield return new[] { typeof(TimeSpan) };
            yield return new[] { typeof(string) };
            yield return new[] { typeof(int) };
        }

        public static IEnumerable<object[]> UnsupportedTypes()
        {
            yield return new[] { typeof(Type) };
            yield return new[] { typeof(ICloneable) };
            yield return new[] { typeof(Activator) };
            yield return new[] { typeof(ApplicationId) };
        }

        public static IEnumerable<object[]> ConvertingData()
        {
            yield return new object[] { typeof(Uri), new Uri("a:/a"), "a:/a" };
            yield return new object[] { typeof(A), new A("value"), "value" };
            yield return new object[] { typeof(TimeSpan), new TimeSpan(1, 20, 0), "1:20" };
        }

        private class A
        {
            public A(string s) { this.s = s; }
            private readonly string s;
            public override bool Equals(object obj)
            {
                var a = obj as A;
                return a.s?.Equals(s) ?? false;
            }
            public override int GetHashCode() => base.GetHashCode();
        }
    }
}
