using System;
using System.Collections.Generic;

namespace SimpleCommandLine.Parsing
{
    /// <summary>
    /// Base interface for all converters.
    /// </summary>
    public interface IConverter { }

    /// <summary>
    /// Provides a method for converting a string to a specified type.
    /// </summary>
    public interface ISingleValueConverter : IConverter
    {
        /// <summary>
        /// Converts a given string to a specified type.
        /// </summary>
        /// <param name="value">A string to convert.</param>
        /// <param name="formatProvider"></param>
        /// <returns>A converted object.</returns>
        ParsingResult Convert(string value, IFormatProvider formatProvider);
        ParsingResult DefaultValue => null;
    }

    internal interface IMultipleValueConverter : IConverter
    {
        ParsingResult Convert(IReadOnlyList<object> values);
        IEnumerable<IConverter> ElementConverters { get; }
    }
}