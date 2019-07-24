using System;

namespace SimpleCommandLine.Parsing
{
    internal interface ICollectionConvertersFactory
    {
        CollectionConverter GetConverter(Type type);
    }
}