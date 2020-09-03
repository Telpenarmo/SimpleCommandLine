using System;
using System.Collections.Generic;
using SimpleCommandLine.Registration;

namespace SimpleCommandLine.Tests.Fakes
{
    internal class TestObject
    {
        public string FirstValue { get; set; }
        public int SecondValue { get; set; }
        public string[] ArrayValue { get; set; }
        public string StringOption { get; set; }
        public bool BoolOption1 { get; set; }
        public bool BoolOption2 { get; set; }
        public bool BoolOption3 { get; set; }
        public string[] ArrayOption { get; set; }

        public static Func<TestObject> Factory { get; } = new Func<TestObject>(() => new TestObject());

        public static IEnumerable<ValueInfo> GetValues(bool limited)
        {
            yield return Value(nameof(FirstValue), 1);
            yield return Value(nameof(SecondValue), 2);
            yield return limited
                ? Value(nameof(ArrayValue), new ValueAttribute(2) { Maximum = 2 })
                : Value(nameof(ArrayValue), 2);
        }

        public static IEnumerable<OptionInfo> Options = new[]
            {
                Option(nameof(StringOption), new OptionAttribute { LongName = "opt", ShortName = 'o' }),
                Option(nameof(BoolOption1), new OptionAttribute { LongName = "first", ShortName = '1' }),
                Option(nameof(BoolOption2), new OptionAttribute { LongName = "second", ShortName = '2' }),
                Option(nameof(BoolOption3), new OptionAttribute { LongName = "third", ShortName = '3' }),
                Option(nameof(ArrayOption), new OptionAttribute { LongName = "arropt" }),
            };

        private static OptionInfo Option(string propertyName, OptionAttribute attribute)
        {
            var property = typeof(TestObject).GetProperty(propertyName);
            return new OptionInfo(property.PropertyType, property.SetValue, attribute);
        }

        private static ValueInfo Value(string propertyName, uint index)
        {
            var property = typeof(TestObject).GetProperty(propertyName);
            return new ValueInfo(property.PropertyType, property.SetValue, new ValueAttribute(index));
        }

        private static ValueInfo Value(string propertyName, ValueAttribute attribute)
        {
            var property = typeof(TestObject).GetProperty(propertyName);
            return new ValueInfo(property.PropertyType, property.SetValue, attribute);
        }
    }

    [Flags]
    public enum AnEnum
    {
        first,
        second,
        third,
        another
    }
}
