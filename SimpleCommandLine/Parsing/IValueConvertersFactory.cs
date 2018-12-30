namespace SimpleCommandLine.Parsing
{
    internal interface IValueConvertersFactory
    {
        IValueConverter GetConverter(Registration.ParsingArgumentInfo argumentInfo);
        bool CanConvert(System.Type type);
        void Register(IValueConverter converter, System.Type type);
    }
}