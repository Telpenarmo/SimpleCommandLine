namespace SimpleCommandLine.Registration
{
    internal interface ITypeRegisterer
    {
        ParsingTypeInfo Register<T>(System.Func<T> factory);
    }
}