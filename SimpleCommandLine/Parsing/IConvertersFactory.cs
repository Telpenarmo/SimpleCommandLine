using System;

namespace SimpleCommandLine.Parsing
{
    internal interface IConvertersFactory
    {
        IConverter GetConverter(Type argumentInfo);
        void RegisterConverter(IValueConverter converter, Type type);
    }
}