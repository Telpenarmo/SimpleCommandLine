using System;
using System.Linq;

namespace SimpleCommandLine
{
    /// <summary>
    /// Marks a property as an command-line option.
    /// </summary>
    public class OptionAttribute : ArgumentAttribute
    {
        private string shortName;
        /// <summary>
        /// Defines the character by providing which the user can refer to the current option.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the given value is invalid.</exception>
        public string ShortName
        {
            get => shortName;
            set => shortName = value switch
            {
                string _ when value.Length != 1 =>
                    throw new ArgumentException($"{nameof(ShortName)} value must be exactly one sign."),
                string _ when char.IsLetterOrDigit(value[0]) => value,
                _ => throw new ArgumentException($"{nameof(ShortName)} value must be a letter or a digit.")
            };
        }

        private string longName;
        /// <summary>
        /// Defines the character by providing which the user can refer to the current option.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the given value is invalid.</exception>
        public string LongName
        {
            get => longName;
            set => longName = !value.Any(x => char.IsWhiteSpace(x)) ? value
                : throw new ArgumentException($"{nameof(LongName)} value must not contain any white spaces.");
        }
    }
}
