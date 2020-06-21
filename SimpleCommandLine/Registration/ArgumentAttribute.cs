using System;

namespace SimpleCommandLine
{
    /// <summary>
    /// Base class for attributes defining arguments of command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ArgumentAttribute : Attribute
    {
        /// <summary>
        /// When this argument is a collection, gets or sets the maximal number of values it may get; otherwise ignored.
        /// </summary>
        public virtual int Maximum { get; set; }
    }
}
