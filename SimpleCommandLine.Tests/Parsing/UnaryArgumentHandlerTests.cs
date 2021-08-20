using SimpleCommandLine.Parsing;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tests.Fakes;
using Xunit;
using static System.Globalization.CultureInfo;

namespace SimpleCommandLine.Tests.Parsing
{
    public class UnaryArgumentHandlerTests
    {
        private UnaryArgumentHandler NewParserWithoutDefaultValue()
            => new(new ParameterInfo(typeof(object), (x, y) => { }), new FakeConverter(), InvariantCulture);
        private UnaryArgumentHandler NewParserWithDefaultValue()
            => new(new ParameterInfo(typeof(object), (x, y) => { }), new SwitchConverter(), InvariantCulture);

        [Fact]
        public void With_converter_not_implementing_DefaultValue_new_instance_has_both_AcceptValue_and_RequiresValue_true()
        {
            var instance = NewParserWithoutDefaultValue();
            Assert.True(instance.AcceptsValue);
            Assert.True(instance.RequiresValue);
        }

        [Fact]
        public void With_converter_implementing_DefaultValue_new_instance_has_both_AcceptValue_and_RequiresValue_false()
        {
            var instance = NewParserWithDefaultValue();
            Assert.False(instance.AcceptsValue);
            Assert.False(instance.RequiresValue);
        }

        [Fact]
        public void With_any_converter_after_SetValue_call_instance_has_both_AcceptValue_and_RequiresValue_false()
        {
            var instance = NewParserWithoutDefaultValue();
            instance.SetValue(new Tokens.ValueToken(""));
            Assert.False(instance.AcceptsValue);
            Assert.False(instance.RequiresValue);

            instance = NewParserWithDefaultValue();
            instance.SetValue(new Tokens.ValueToken(""));
            Assert.False(instance.AcceptsValue);
            Assert.False(instance.RequiresValue);
        }

        [Fact]
        public void With_value_not_given_nor_default_Parse_returns_error()
        {
            var instance = NewParserWithoutDefaultValue();
            var result = instance.GetResult();
            Assert.True(result.IsError);
            Assert.Null(result.ResultObject);
        }

        [Fact]
        public void With_value_given_Parse_returns_success()
        {
            var instance = NewParserWithoutDefaultValue();
            instance.AddValue(new Tokens.ValueToken(""));
            var result = instance.GetResult();
            Assert.False(result.IsError);

            instance = NewParserWithoutDefaultValue();
            instance.SetValue(new Tokens.ValueToken(""));
            result = instance.GetResult();
            Assert.False(result.IsError);
        }

        [Fact]
        public void With_value_default_Parse_returns_success()
        {
            var instance = NewParserWithDefaultValue();
            var result = instance.GetResult();

            Assert.False(result.IsError);
        }

        [Fact]
        public void With_value_default_and_set_parses_the_given()
        {
            var instance = NewParserWithDefaultValue();
            var arg = "non-default";
            instance.SetValue(new Tokens.ValueToken(arg));
            var result = instance.GetResult();

            Assert.Equal(arg, result.ResultObject as string);
        }
    }
}