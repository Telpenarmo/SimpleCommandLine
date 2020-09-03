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
        [Fact]
        public void With_registered_converter_requested_returns_it()
        {
            var instance = new ConvertersFactory();
            var converter = new FakeConverter();
            instance.RegisterConverter(converter, typeof(object));
            var result = instance.GetConverter(typeof(object));
            Assert.Same(converter, result);
        }

        [Fact]
        public void With_non_registered_and_not_supported_type_requested_returns_null()
        {
            var instance = new ConvertersFactory();
            var result = instance.GetConverter(typeof(object));
            Assert.Null(result);
        }

        [Fact]
        public void With_tuple_of_registered_type_requested_returns_valid()
        {
            var instance = new ConvertersFactory();
            instance.RegisterConverter(new FakeConverter(), typeof(object));
            var result = instance.GetConverter(typeof(Tuple<object, object>));
            Assert.True(typeof(Tuple<object, object>).IsTuple() );
            Assert.IsType<SimpleCommandLine.Parsing.Converters.TupleConverter>(result);
        }

        [Theory, MemberData(nameof(SupportedCollections))]
        public void With_supported_collection_of_registered_type_requested_returns_valid(Type type)
        {
            var instance = new ConvertersFactory();
            instance.RegisterConverter(new FakeConverter(), typeof(object));
            var result = instance.GetConverter(type);
            Assert.IsAssignableFrom<IMultipleValueConverter>(result);
            var converted = (result as IMultipleValueConverter).Convert(new[] { "one", "two", "three" }).ResultObject;
            Assert.IsAssignableFrom(type, converted);
        }

        [Fact]
        public void With_dictionary_of_registered_type_requested_returns_valid()
        {
            var instance = new ConvertersFactory();
            instance.RegisterConverter(new FakeConverter(), typeof(object));
            var result = instance.GetConverter(typeof(Dictionary<object, object>));
            Assert.IsAssignableFrom<IMultipleValueConverter>(result);
            var array = (result as IMultipleValueConverter).Convert(
                new object[] {
                    KeyValuePair.Create<object, object>("left", "right"),
                    KeyValuePair.Create<object, object>("up", "down") }).ResultObject;
            Assert.IsAssignableFrom<Dictionary<object, object>>(array);
        }

        [Fact]
        public void With_array_of_unregistered_type_requested_returns_null()
        {

            var instance = new ConvertersFactory();
            var result = instance.GetConverter(typeof(object[]));
            Assert.Null(result);
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
            public CustomCollection(IEnumerable<object> args) { }

            public IEnumerator<object> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}