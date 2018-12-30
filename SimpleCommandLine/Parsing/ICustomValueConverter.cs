using System;

namespace SimpleCommandLine.Parsing
{
    /// <summary>
    /// Provides a method for converting a string to a specified type.
    /// </summary>
    public interface IValueConverter
    {
        /// <summary>
        /// Converts a given string to a specified type.
        /// </summary>
        /// <param name="str">A string to tokenize.</param>
        /// <param name="formatProvider"></param>
        /// <returns>A converted object.</returns>
        object Convert(string str, IFormatProvider formatProvider);
    }

    /// <summary>
    /// Provides a method for converting a string to <typeparamref name="T"/> type.
    /// </summary>
    public interface IValueConverter<out T> : IValueConverter
    {
        /// <summary>
        /// Converts a given string to <typeparamref name="T"/> type.
        /// </summary>
        /// <param name="str">A string to tokenize.</param>
        /// <param name="formatProvider"></param>
        /// <returns>A converted object.</returns>
        new T Convert(string str, IFormatProvider formatProvider);
    }
}