using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing.Converters
{
    internal class GenericCollectionConverter : ArrayConverter
    {
        private readonly Type type;

        public GenericCollectionConverter(Type type, IValueConverter valueConverter)
            : base(type.GenericTypeArguments[0], valueConverter)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public override bool Convert(IReadOnlyList<string> values, IFormatProvider formatProvider, out object result)
        {
            result = null;
            if (!base.Convert(values, formatProvider, out object array))
                return false;
            result = Activator.CreateInstance(type, new object[] { array });
            return true;
        }
    }
}