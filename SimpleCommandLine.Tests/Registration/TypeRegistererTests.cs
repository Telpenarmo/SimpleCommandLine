using System;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tests.Fakes;
using Xunit;

namespace SimpleCommandLine.Tests.Registration
{
    public class TypeRegistererTests
    {
        public TypeRegistererTests()
        {
            StringConvertingFactory = new FakeConvertersFactory();
            StringConvertingFactory.RegisterConverter(new FakeConverter(), typeof(string));
        }

        private SimpleCommandLine.Parsing.IConvertersFactory StringConvertingFactory { get; }

        [Fact]
        public void Produced_factory_executes_registered_function()
        {
            var convertersFactory = new FakeConvertersFactory();
            var expected = new object();
            var executed = false;

            var instance = new TypeRegisterer<object>(() => { executed = true; return expected; }, convertersFactory);
            var info = instance.Build();
            var produced = info.Factory();

            Assert.True(executed);
            Assert.Same(expected, produced);
        }

        [Fact]
        public void Building_after_registering_parameter_of_nonconvertible_type_fails()
        {
            var convertersFactory = new FakeConvertersFactory();
            var instance = new TypeRegisterer<object>(() => new(), convertersFactory);
            instance.RegisterValue<string>((_, _) => { }, 0);
            Assert.ThrowsAny<Exception>(() => instance.Build());
        }

        [Fact]
        public void Building_after_registering_parameter_of_convertible_type_succeeds()
        {
            var instance = new TypeRegisterer<object>(() => new(), StringConvertingFactory);
            instance.RegisterValue<string>((_, _) => { }, 0);
        }

        [Fact]
        public void ProducedAliasesAreEqualToRegistered()
        {
            var convertersFactory = new FakeConvertersFactory();
            var instance = new TypeRegisterer<object>(() => new(), convertersFactory);

            var aliases = new[] { "one", "two", "three" };
            instance.AsCommand(aliases);
            var res = instance.Build();

            Assert.Equal(aliases, res.Aliases);
        }

        [Fact]
        public void Registering_value_of_same_index_twice_overrides()
        {
            var theOld = false;
            var theNew = false;

            var instance = new TypeRegisterer<object>(() => new(), StringConvertingFactory);
            instance.RegisterValue<string>((_, _) => theOld = true, 0)
                .RegisterValue<string>((_, _) => theNew = true, 0);
            instance.Build().Values[0].SetValue(new(), "");

            Assert.False(theOld);
            Assert.True(theNew);
        }

        [Fact]
        public void Loading_two_attributes_of_same_index_throws()
        {
            var instance = new TypeRegisterer<FaultyArgsDeclaration>(() => new(), new FakeConvertersFactory());
            instance.UseAttributes();
            Assert.Throws<InvalidOperationException>(instance.Build);
        }

        [Fact]
        public void Registering_value_with_index_same_as_attribute_overrides()
        {
            var overriding = false;
            var target = new FineArgsDeclaration();
            var arg = "something";

            var registerer = new TypeRegisterer<FineArgsDeclaration>(() => new(), StringConvertingFactory);
            registerer.RegisterValue<string>((_, _) => overriding = true, 1);
            var info = registerer.Build();
            info.Values[0].SetValue(target, arg);

            Assert.Single(info.Values);
            Assert.True(overriding);
            Assert.NotEqual(arg, target.Value);
        }

        private class FineArgsDeclaration
        {
            [Value(1)]
            public string Value { get; set; }

            [Option(LongName = "option")]
            public string Option { get; set; }
        }

        private class FaultyArgsDeclaration
        {
            [Value(1)]
            public string FirstValue { get; set; }

            [Value(1)]
            public string SecondValue { get; set; }

        }
    }
}