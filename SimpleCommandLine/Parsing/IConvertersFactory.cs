using System;

namespace SimpleCommandLine.Parsing
{
    internal interface IConvertersFactory
    {
        IConverter<object> GetConverter(Type argumentInfo);
        void RegisterConverter(IValueConverter converter, Type type);
    }
}