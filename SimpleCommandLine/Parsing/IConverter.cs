using System;

namespace SimpleCommandLine.Parsing
{
    /// <summary>
    /// Base interface for all converters.
    /// </summary>
    public interface IConverter { }

    /// <summary>
    /// Provides a method for converting a string to a specified type.
    /// </summary>
    public interface IValueConverter : IConverter
    {
        /// <summary>
        /// Converts a given string to a specified type.
        /// </summary>
        /// <param name="value">A string to convert.</param>
        /// <param name="formatProvider"></param>
        /// <returns>A converted object.</returns>
        object Convert(string value, IFormatProvider formatProvider);
    }

    /// <summary>
    /// Provides a method for converting a string to <typeparamref name="T"/> type.
    /// </summary>
    public interface IValueConverter<out T> : IValueConverter
    {
        /// <summary>
        /// Converts a given string to <typeparamref name="T"/> type.
        /// </summary>
        /// <param name="value">A string to convert.</param>
        /// <param name="formatProvider"></param>
        /// <returns>A converted object.</returns>
        new T Convert(string value, IFormatProvider formatProvider);
    }
}