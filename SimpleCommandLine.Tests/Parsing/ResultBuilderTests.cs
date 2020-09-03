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
            => NewIstanceWith(Empty<ValueInfo>(), TestObject.Options);
        private ResultBuilder NewEmptyInstance()
            => NewIstanceWith(Empty<ValueInfo>(), Empty<OptionInfo>());
        private ResultBuilder NewLimitedValuesInstance()
            => NewIstanceWith(TestObject.GetValues(true), Empty<OptionInfo>());
        private ResultBuilder NewUnlimitedValuesInstance()
            => NewIstanceWith(TestObject.GetValues(false), Empty<OptionInfo>());

        private ResultBuilder NewIstanceWith(IEnumerable<ValueInfo> values, IEnumerable<OptionInfo> options)
        {
            ConvertersFactory convertersFactory = new ConvertersFactory();
            convertersFactory.RegisterConverter(new FakeConverter(), typeof(string));
            return new ResultBuilder(new TypeInfo(values, options, TestObject.Factory), convertersFactory, InvariantCulture);
        }

        #region TryAddOption
        [Fact]
        public void Given_valid_option_TryAddOption_returns_true()
        {
            var result = NewValuelessInstance().TryAddOption(singleOption);
            Assert.True(result);
        }

        [Fact]
        public void Given_invalid_option_TryAddOption_returns_false()
        {
            var result = NewValuelessInstance().TryAddOption(invalidOption);
            Assert.False(result);
        }

        [Fact]
        public void Given_the_same_valid_option_twice_TryAddOption_returns_true()
        {
            var instance = NewValuelessInstance();
            instance.TryAddOption(singleOption);
            Assert.True(instance.TryAddOption(singleOption));
        }
        #endregion

        #region LastAssignedOption
        [Fact]
        public void Given_fresh_instance_LastAssignedOption_is_null()
        {
            var result = NewValuelessInstance().LastAssignedOption;
            Assert.Null(result);
        }

        [Fact]
        public void After_successfull_option_addition_LastAssignedOption_is_not_null()
        {
            var instance = NewValuelessInstance();
            instance.TryAddOption(singleOption);
            Assert.NotNull(instance.LastAssignedOption);
        }

        [Fact]
        public void After_only_value_addition_LastAssignedOption_is_null()
        {
            var instance = NewUnlimitedValuesInstance();
            instance.AddValue(new ValueToken("val"));
            var result = instance.LastAssignedOption;
            Assert.Null(result);
        }
        #endregion

        #region AwaitsValue 
        [Fact]
        public void With_type_without_values_AwaitsValue_is_false()
        {
            var instance = NewEmptyInstance();
            var result = instance.AwaitsValue;
            Assert.False(result);
        }

        [Fact]
        public void After_reaching_maximal_values_number_AwaitsValue_is_false()
        {
            var instance = NewLimitedValuesInstance();
            for (int i = 0; i < 4; i++)
                instance.AddValue(new ValueToken(""));
            Assert.False(instance.AwaitsValue);
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