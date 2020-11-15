using System;
using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing.Converters;
using System.Collections;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public class TupleConverterTests
    {
        readonly SimpleCommandLine.Parsing.IConverter[] elementConverters
            = new SimpleCommandLine.Parsing.IConverter[] { };

        [Theory, MemberData(nameof(SupportedTypes))]
        public void Given_supported_type_converts_well(Type type, Predicate<object> check)
        {
            var instance = new TupleConverter(type, 2, elementConverters);
            var result = instance.Convert(new[] { "left", "right" });
            Assert.False(result.IsError);
            Assert.IsType(type, result.ResultObject);
            Assert.True(check(result.ResultObject));
        }

        [Fact]
        public void Given_wrong_number_of_values_returns_error()
        {
            var instance = new TupleConverter(typeof(Tuple<string, string, string>), 3, elementConverters);
            var result = instance.Convert(new[] { "left", "right" });
            Assert.True(result.IsError);
            result = instance.Convert(new[] { "first", "second", "third", "fourth" });
            Assert.True(result.IsError);
        }

        public static IEnumerable<object[]> SupportedTypes()
        {
            yield return SupportedType<ValueTuple<string, string>>(
                tuple => tuple.Item1 == "left" && tuple.Item2 == "right");
            yield return SupportedType<Tuple<string, string>>(
                tuple => tuple.Item1 == "left" && tuple.Item2 == "right");
            yield return SupportedType<KeyValuePair<string, string>>(
                pair => pair.Key == "left" && pair.Value == "right");
            yield return SupportedType<DictionaryEntry>(
                pair => (string)pair.Key == "left" && (string)pair.Value == "right");
        }

        private static object[] SupportedType<T>(Predicate<T> check)
            => new object[]
            {
                typeof(T),
                new Predicate<object>(o => check((T)o)),
            };
    }
}