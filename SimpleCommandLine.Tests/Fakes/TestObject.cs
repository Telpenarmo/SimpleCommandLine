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

        public static IReadOnlyList<ParameterInfo> GetValues(bool limited)
            => new ParameterInfo[]
            {
                Value(nameof(FirstValue)),
                limited
                    ? Value(nameof(ArrayValue), new ValueAttribute(2) { Maximum = ValuesLimit - 1 })
                    : Value(nameof(ArrayValue)),
            };

        public static int ValuesLimit => 3;
        public static IReadOnlyDictionary<string, ParameterInfo> Options
            = new Dictionary<string, ParameterInfo>
            {
                { "opt", Option(nameof(StringOption)) },
                { "first", Option(nameof(BoolOption1)) },
                { "1", Option(nameof(BoolOption1)) },
                { "2", Option(nameof(BoolOption2)) },
                { "3", Option(nameof(BoolOption3)) },
                { "arropt", Option(nameof(ArrayOption)) },
            };

        private static ParameterInfo Option(string propertyName)
        {
            var property = typeof(TestObject).GetProperty(propertyName);
            return new ParameterInfo(property.PropertyType, property.SetValue);
        }

        private static ParameterInfo Value(string propertyName)
        {
            var property = typeof(TestObject).GetProperty(propertyName);
            return new ParameterInfo(property.PropertyType, property.SetValue);
        }

        private static ParameterInfo Value(string propertyName, ValueAttribute attribute)
        {
            var property = typeof(TestObject).GetProperty(propertyName);
            return new ParameterInfo(property.PropertyType, property.SetValue, attribute);
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
