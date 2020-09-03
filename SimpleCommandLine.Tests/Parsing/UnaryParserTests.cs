using Xunit;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tests.Fakes;
using static System.Globalization.CultureInfo;

namespace SimpleCommandLine.Tests.Parsing
{
    public class UnaryParserTests
    {
        private UnaryParser NewParserWithoutDefaultValue()
            => new UnaryParser(new FakeArgumentInfo(typeof(object)), new FakeConverter(), InvariantCulture);
        private UnaryParser NewParserWithDefaultValue()
            => new UnaryParser(new FakeArgumentInfo(typeof(object)), new SwitchConverter(), InvariantCulture);

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
            var result = instance.Parse(new object());
            Assert.True(result.IsError);
            Assert.Null(result.ResultObject);
        }

        [Fact]
        public void With_value_given_Parse_returns_success()
        {
            var instance = NewParserWithoutDefaultValue();
            instance.AddValue(new Tokens.ValueToken(""));
            var target = new object();
            var result = instance.Parse(target);
            Assert.False(result.IsError);

            instance = NewParserWithoutDefaultValue();
            instance.SetValue(new Tokens.ValueToken(""));
            result = instance.Parse(target);
            Assert.False(result.IsError);
        }

        [Fact]
        public void With_value_default_Parse_returns_success()
        {
            var instance = NewParserWithDefaultValue();
            var target = new object();
            var result = instance.Parse(target);

            Assert.False(result.IsError);
        }

        [Fact]
        public void With_value_default_and_set_parses_the_given()
        {
            var instance = NewParserWithDefaultValue();
            var arg = "non-default";
            instance.SetValue(new Tokens.ValueToken(arg));
            var result = instance.Parse(new object());

            Assert.Equal(arg, result.ResultObject as string);
        }
    }
}