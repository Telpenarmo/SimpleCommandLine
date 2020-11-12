using System.Collections.Generic;
using Xunit;
using SimpleCommandLine.Tests.Fakes;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokens;
using static System.Globalization.CultureInfo;
using static System.Linq.Enumerable;

namespace SimpleCommandLine.Tests.Parsing
{
    public class ResultBuilderTests
    {
        private ResultBuilder NewValuelessInstance()
            => NewIstanceWith(new ParameterInfo[] { }, TestObject.Options);
        private ResultBuilder NewEmptyInstance()
            => NewIstanceWith(new ParameterInfo[] { }, new Dictionary<string, ParameterInfo>());
        private ResultBuilder NewLimitedValuesInstance()
            => NewIstanceWith(TestObject.GetValues(true), new Dictionary<string, ParameterInfo>());
        private ResultBuilder NewUnlimitedValuesInstance()
            => NewIstanceWith(TestObject.GetValues(false), new Dictionary<string, ParameterInfo>());

        private ResultBuilder NewIstanceWith(IReadOnlyList<ParameterInfo> values, IReadOnlyDictionary<string, ParameterInfo> options)
        {
            ConvertersFactory convertersFactory = new ConvertersFactory();
            convertersFactory.RegisterConverter(new FakeConverter(), typeof(string));
            convertersFactory.RegisterConverter(new FakeImplicitConverter(), typeof(bool));
            convertersFactory.CheckForType(typeof(string[]));
            return new ResultBuilder(new TypeInfo(values, options, TestObject.Factory), convertersFactory, InvariantCulture);
        }

        #region Options
        [Fact]
        public void Given_explicit_option_Returns_error()
        {
            var obj = NewValuelessInstance();
            obj.HandleToken(singleOption);
            var result = obj.Build();
            Assert.Equal("Value not set.", result.ErrorMessages.Single());
        }

        [Fact]
        public void Given_implicit_option_Sets_default()
        {
            var obj = NewValuelessInstance();
            obj.HandleToken(boolOption);
            var result = obj.Build();
            Assert.IsType<TestObject>(result.ResultObject);
            Assert.True((result.ResultObject as TestObject).BoolOption1);
        }

        [Fact]
        public void Given_explicit_option_and_value_Returns_success()
        {
            var obj = NewValuelessInstance();
            obj.HandleToken(singleOption);
            obj.HandleToken(new ValueToken(""));
            var result = obj.Build();
            Assert.False(result.IsError);
        }

        [Fact]
        public void Given_assigned_implicit_option_Sets_assignment()
        {
            var obj = NewValuelessInstance();
            obj.HandleToken(new AssignedValueToken(boolOption, new ValueToken("false")));
            var result = obj.Build();
            Assert.IsType<TestObject>(result.ResultObject);
            Assert.False((result.ResultObject as TestObject).BoolOption1);
        }

        [Fact]
        public void Given_assigned_option_and_value_sets_assignment_and_first_value()
        {
            var obj = NewIstanceWith(TestObject.GetValues(false), TestObject.Options);
            obj.HandleToken(new AssignedValueToken(boolOption, new ValueToken("true")));
            obj.HandleToken(new ValueToken("abc"));
            var result = obj.Build();
            Assert.IsType<TestObject>(result.ResultObject);
            Assert.True((result.ResultObject as TestObject).BoolOption1);
            Assert.Equal("abc", (result.ResultObject as TestObject).FirstValue);
        }

        [Fact]
        public void Given_group_of_implicit_options_Sets_them_all()
        {
            var obj = NewValuelessInstance();
            obj.HandleToken(new OptionsGroupToken(
                new[] { new OptionToken("1"), new OptionToken("2"), new OptionToken("3") }));
            var result = obj.Build();
            Assert.IsType<TestObject>(result.ResultObject);
            Assert.True((result.ResultObject as TestObject).BoolOption1);
            Assert.True((result.ResultObject as TestObject).BoolOption2);
            Assert.True((result.ResultObject as TestObject).BoolOption3);
        }

        [Fact]
        public void Given_invalid_option_Returns_error()
        {
            var obj = NewEmptyInstance();
            obj.HandleToken(invalidOption);
            var result = obj.Build();
            Assert.Equal($"The current type does not contain the \"{invalidOption.Value}\" option.",
                         result.ErrorMessages.Single());
        }

        [Fact]
        public void Given_the_same_valid_option_twice_Sets_the_latter()
        {
            var obj = NewValuelessInstance();
            obj.HandleToken(singleOption);
            obj.HandleToken(new ValueToken("aa"));
            obj.HandleToken(singleOption);
            obj.HandleToken(new ValueToken("bb"));
            var result = obj.Build();
            Assert.IsType<TestObject>(result.ResultObject);
            Assert.Equal("bb", (result.ResultObject as TestObject).StringOption);
        }

        [Fact]
        public void Setting_the_same_valid_option_twice_Returns_one_error()
        {
            var obj = NewValuelessInstance();
            obj.HandleToken(singleOption);
            obj.HandleToken(singleOption);
            var result = obj.Build();
            Assert.Equal(2, result.ErrorMessages.Count());
        }
        #endregion

        #region Values
        [Fact]
        public void Given_maximal_values_number_Returns_success()
        {
            var obj = NewLimitedValuesInstance();
            for (int i = 0; i < TestObject.ValuesLimit; i++)
                obj.HandleToken(new ValueToken(""));
            var result = obj.Build();
            Assert.False(result.IsError);
        }

        [Fact]
        public void With_limited_values_number_Given_too_many_values_Returns_failure()
        {
            var obj = NewLimitedValuesInstance();
            for (int i = 0; i < TestObject.ValuesLimit + 1; i++)
                obj.HandleToken(new ValueToken(""));
            var result = obj.Build();
            Assert.True(result.IsError);
        }

        [Fact]
        public void With_unlimited_values_number_Given_many_values_Returns_success()
        {
            var obj = NewUnlimitedValuesInstance();
            for (int i = 0; i < 30; i++)
                obj.HandleToken(new ValueToken(""));
            var result = obj.Build();
            Assert.False(result.IsError);
        }
        #endregion

        #region Helpers
        private static readonly OptionToken singleOption = new OptionToken("opt");
        private static readonly OptionToken boolOption = new OptionToken("1");
        private static readonly OptionToken multiOption = new OptionToken("arropt");
        private static readonly OptionToken invalidOption = new OptionToken("bad");
        #endregion
    }
}