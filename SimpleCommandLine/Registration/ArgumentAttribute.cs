using System;

namespace SimpleCommandLine
{
    /// <summary>
    /// Base class for attributes defining arguments of command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ArgumentAttribute : Attribute
    {
        
    }
}
