using System;
using Xunit;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;
using SimpleCommandLine.Tests.Fakes;
using static System.Globalization.CultureInfo;

namespace SimpleCommandLine.Tests.Parsing
{
    public class CollectionHandlerTests
    {
        private CollectionHandler NewInstance(ParameterAttribute attribute)
            => new CollectionHandler(new ParameterInfo(typeof(object[]), (x, y) => { }, attribute),
                new FakeCollectionConverter(), InvariantCulture);

        [Fact]
        public void With_no_minimal_values_number_RequiresValue_is_always_false()
        {
            var instance = NewInstance(new FakeArgumentAttribute());
            Assert.False(instance.RequiresValue);
            instance.AddValue(new ValueToken(""));
            Assert.False(instance.RequiresValue);
        }

        [Fact]
        public void With_nonzero_minimal_values_number_RequiresValue_is_true_at_beginning()
        {
            var instance = NewInstance(new FakeArgumentAttribute() { Minimum = 2 });
            Assert.True(instance.RequiresValue);
        }

        [Fact]
        public void AcceptsValue_is_true_at_beginning()
        {
            var instance = NewInstance(new FakeArgumentAttribute());
            Assert.True(instance.AcceptsValue);
            instance = NewInstance(new FakeArgumentAttribute() { Minimum = 2 });
            Assert.True(instance.AcceptsValue);
            instance = NewInstance(new FakeArgumentAttribute() { Maximum = 2 });
            Assert.True(instance.AcceptsValue);
            instance = NewInstance(new FakeArgumentAttribute() { Minimum = 2, Maximum = 4 });
            Assert.True(instance.AcceptsValue);
        }

        [Fact]
        public void RequiresValue_is_false_after_adding_at_least_minimal_number_of_values()
        {
            var instance = NewInstance(new FakeArgumentAttribute { Minimum = 2 });
            instance.AddValue(new ValueToken(""));
            Assert.True(instance.RequiresValue);
            instance.AddValue(new ValueToken(""));
            Assert.False(instance.RequiresValue);
        }

        [Fact]
        public void RequiresValue_is_false_after_setting_ValuesGroup()
        {
            var instance = NewInstance(new FakeArgumentAttribute { Minimum = 3 });
            instance.SetValue(new ValuesGroupToken(new[] { new ValueToken("a"), new ValueToken("b") }, ""));
            Assert.False(instance.RequiresValue);
        }

        [Fact]
        public void AcceptsValue_is_false_after_adding_maximal_number_of_values()
        {
            var instance = NewInstance(new FakeArgumentAttribute { Maximum = 2 });
            instance.AddValue(new ValueToken(""));
            Assert.True(instance.AcceptsValue);
            instance.AddValue(new ValueToken(""));
            Assert.False(instance.AcceptsValue);
        }

        [Fact]
        public void AcceptsValue_is_false_after_setting_ValuesGroup()
        {
            var instance = NewInstance(new FakeArgumentAttribute { Maximum = 3 });
            instance.SetValue(new ValuesGroupToken(new[] { new ValueToken("a"), new ValueToken("b") }, ""));
            Assert.False(instance.AcceptsValue);
        }

        [Fact]
        public void After_setting_value_AddValue_throws()
        {
            var instance = NewInstance(new FakeArgumentAttribute());
            instance.SetValue(new ValuesGroupToken(new[] { new ValueToken("a"), new ValueToken("b") }, ""));
            Assert.Throws<InvalidOperationException>(() => instance.AddValue(new ValueToken("")));
        }

        [Fact]
        public void After_setting_value_SetValue_throws()
        {
            var instance = NewInstance(new FakeArgumentAttribute());
            var token = new ValuesGroupToken(new[] { new ValueToken("a"), new ValueToken("b") }, "");
            instance.SetValue(token);
            Assert.Throws<InvalidOperationException>(() => instance.SetValue(token));
        }
    }
}