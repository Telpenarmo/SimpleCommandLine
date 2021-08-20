using System;
using System.Collections.Generic;
using SimpleCommandLine.Parsing.Converters;
using SimpleCommandLine.Tests.Fakes;
using Xunit;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public class ArrayConverterTests
    {
        private static ArrayConverter NewInstance<T>() => new(typeof(T), new FakeConverter());

        [Fact]
        public void Given_array_of_valid_objects_returns_success()
        {
            var conv = NewInstance<string>();
            string[] input = new[] { "first", "second" };

            var res = conv.Convert(input);
            Assert.False(res.IsError);
            var obj = res.ResultObject;
            Assert.IsType<string[]>(obj);
            var arr = obj as string[];
            Assert.Equal(2, arr.Length);
            Assert.Same(input[0], arr[0]);
        }

        [Fact]
        public void Given_list_of_valid_objects_returns_success()
        {
            var conv = NewInstance<string>();
            List<string> input = new List<string> { "first", "second" };

            var res = conv.Convert(input);
            Assert.False(res.IsError);
            var obj = res.ResultObject;
            Assert.IsType<string[]>(obj);
            var arr = obj as string[];
            Assert.Equal(2, arr.Length);
            Assert.Same(input[0], arr[0]);
        }

        [Fact]
        public void Given_untyped_array_of_valid_objects_returns_success()
        {
            var conv = NewInstance<string>();
            object[] input = new object[] { "1", "2" };

            var res = conv.Convert(input);
            Assert.False(res.IsError);
            var obj = res.ResultObject;
            Assert.IsType<string[]>(obj);
            var arr = obj as string[];
            Assert.Equal(2, arr.Length);
            Assert.Same(input[0], arr[0]);
        }

        [Fact]
        public void Given_array_of_invalid_objects_returns_success()
        {
            var conv = NewInstance<string>();
            var input = new object[] { 1, 2 };
            Assert.Throws<ArgumentException>(() => conv.Convert(input));
        }

        [Fact]
        public void Given_array_of_inherited_type_returns_success()
        {
            var conv = NewInstance<Foo>();
            Bar[] input = new[] { new Bar() };

            var res = conv.Convert(input);
            Assert.False(res.IsError);
            Assert.IsType<Foo[]>(res.ResultObject);
            var arr = res.ResultObject as Foo[];
            Assert.Single(arr);
            Assert.Same(input[0], arr[0]);
        }

        private class Foo { }
        private class Bar : Foo { }
    }
}