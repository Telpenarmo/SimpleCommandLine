namespace SimpleCommandLine.Parsing
{
    internal interface ICollectionConvertersFactory
    {
        CollectionConverter GetConverter(System.Type type);
    }
}