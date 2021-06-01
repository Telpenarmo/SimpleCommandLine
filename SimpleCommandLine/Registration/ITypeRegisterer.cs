using System;

namespace SimpleCommandLine.Registration
{
    public interface ITypeRegisterer<T>
    {
        ITypeRegisterer<T> RegisterOption<TArg>(Action<T?, TArg?> valueSetter, char shortName, string longName);
        ITypeRegisterer<T> RegisterOption<TArg>(Action<T?, TArg?> valueSetter, char shortName);
        ITypeRegisterer<T> RegisterOption<TArg>(Action<T?, TArg?> valueSetter, string longName);
        ITypeRegisterer<T> RegisterValue<TArg>(Action<T?, TArg?> valueSetter, uint index);
        ITypeRegisterer<T> AsCommand(params string[] aliases);
        ITypeRegisterer<T> DiscardAttributes();
        ITypeRegisterer<T> UseAttributes();
    }
}