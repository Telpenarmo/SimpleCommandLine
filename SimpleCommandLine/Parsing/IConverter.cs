﻿using System;

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
        bool Convert(string value, IFormatProvider formatProvider, out object result);
    }
}