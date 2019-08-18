using System;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class EnumConverter : IValueConverter
    {
        private readonly Type enumType;

        public EnumConverter(Type enumType)
        {
            this.enumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
        }

        public object Convert(string value, IFormatProvider formatProvider)
        {
            if (value == null)
                return Enum.ToObject(enumType, 0);
            if (!Enum.IsDefined(enumType, value))
                throw new FormatException($"\"{value}\" is not a valid value. Valid values are: {string.Join(", ", Enum.GetNames(enumType))}.");
            return Enum.Parse(enumType, value, true);
        }
    }
}
