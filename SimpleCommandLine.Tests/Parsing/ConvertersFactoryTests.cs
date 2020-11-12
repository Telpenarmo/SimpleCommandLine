using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tests.Fakes;

namespace SimpleCommandLine.Tests.Parsing
{

    public class ConvertersFactoryTests
    {
        private readonly IFormatProvider culture = System.Globalization.CultureInfo.InvariantCulture;
        
        [Fact]
        public void With_registered_converter_requested_returns_it()
        {
            var instance = new ConvertersFactory();
            var converter = new FakeConverter();
            instance.RegisterConverter(converter, typeof(object));
            var result = instance[typeof(object)];
            Assert.Same(converter, result);
        }

        [Fact]
        public void When_requested_type_is_supported_by_TypeDescriptor_returns_valid()
        {
            var instance = new ConvertersFactory();
            Assert.True(instance.CheckForType(typeof(int)));
            var converter = instance[typeof(int)];
            Assert.IsAssignableFrom<ISingleValueConverter>(converter);
            var conversionResult = (converter as ISingleValueConverter).Convert("42", culture);
            Assert.Equal(42, conversionResult.ResultObject);
        }

        [Fact]
        public void When_requested_type_has_string_constructor_returns_valid()
        {
            var instance = new ConvertersFactory();
            Assert.True(instance.CheckForType(typeof(StringConstruct)));
            var converter = instance[typeof(StringConstruct)];
            Assert.IsAssignableFrom<ISingleValueConverter>(converter);
            var conversionResult = (converter as ISingleValueConverter).Convert("fourty-two", culture);
            Assert.Equal("fourty-two", (conversionResult.ResultObject as StringConstruct)?.S);
        }

        [Fact]
        public void With_non_registered_and_not_supported_type_CheckForType_returns_false()
        {
            var instance = new ConvertersFactory();
            Assert.False(instance.CheckForType(typeof(object[])));
        }

        [Fact]
        public void With_tuple_of_registered_type_requested_returns_valid()
        {
            var instance = new ConvertersFactory();
            instance.RegisterConverter(new FakeConverter(), typeof(object));
            Assert.True(instance.CheckForType(typeof(Tuple<object, object>)));
            var result = instance[typeof(Tuple<object, object>)];
            Assert.True(typeof(Tuple<object, object>).IsTuple());
            Assert.IsType<SimpleCommandLine.Parsing.Converters.TupleConverter>(result);
        }

        [Theory, MemberData(nameof(SupportedCollections))]
        public void With_supported_collection_of_registered_type_requested_returns_valid(Type type)
        {
            var instance = new ConvertersFactory();
            instance.RegisterConverter(new FakeConverter(), typeof(object));
            Assert.True(instance.CheckForType(type));
            var result = instance[type];
            Assert.IsAssignableFrom<IMultipleValueConverter>(result);
            var converted = (result as IMultipleValueConverter).Convert(new[] { "one", "two", "three" }).ResultObject;
            Assert.IsAssignableFrom(type, converted);
        }

        [Fact]
        public void With_dictionary_of_registered_type_requested_returns_valid()
        {
            var instance = new ConvertersFactory();
            instance.RegisterConverter(new FakeConverter(), typeof(object));
            Assert.True(instance.CheckForType(typeof(Dictionary<object, object>)));
            var result = instance[typeof(Dictionary<object, object>)];
            Assert.IsAssignableFrom<IMultipleValueConverter>(result);
            var array = (result as IMultipleValueConverter).Convert(
                new object[] {
                    KeyValuePair.Create<object, object>("left", "right"),
                    KeyValuePair.Create<object, object>("up", "down") }).ResultObject;
            Assert.IsAssignableFrom<Dictionary<object, object>>(array);
        }

        [Fact]
        public void With_array_of_unregistered_type_CheckForType_returns_false()
        {
            var instance = new ConvertersFactory();
            Assert.False(instance.CheckForType(typeof(object[])));
        }

        public static IEnumerable<object[]> SupportedCollections()
        {
            yield return new[] { typeof(object[]) };
            yield return new[] { typeof(List<object>) };
            yield return new[] { typeof(IEnumerable<object>) };
            yield return new[] { typeof(CustomCollection) };
        }

        private class CustomCollection : IEnumerable<object>
        {
            readonly IEnumerable<object> underlying;
            public CustomCollection(IEnumerable<object> underlying) => this.underlying = underlying;
            public IEnumerator<object> GetEnumerator() => underlying.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)underlying).GetEnumerator();
        }

        private class StringConstruct
        {
            public StringConstruct(string s) { S = s; }
            public string S { get; }
        }
    }
}