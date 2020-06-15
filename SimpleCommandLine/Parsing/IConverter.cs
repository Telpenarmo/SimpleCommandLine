using System;

namespace SimpleCommandLine.Parsing
{
    /// <summary>
    /// Base interface for all converters.
    /// </summary>
    public interface IConverter<in TArg>
    {
        bool Convert(TArg arg, IFormatProvider formatProvider, out object result);
    }

    public interface IConverter : IConverter<object> { }

    /// <summary>
    /// Provides a method for converting a string to a specified type.
    /// </summary>
    public interface IValueConverter : IConverter<string>
    {
        /// <summary>
        /// Converts a given string to a specified type.
        /// </summary>
        /// <param name="value">A string to convert.</param>
        /// <param name="formatProvider"></param>
        /// <returns>A converted object.</returns>
    }
}