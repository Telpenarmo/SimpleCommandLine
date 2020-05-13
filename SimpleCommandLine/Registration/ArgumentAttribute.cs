using System;

namespace SimpleCommandLine
{
    /// <summary>
    /// Base class for attributes defining arguments of command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public abstract class ArgumentAttribute : Attribute
    {
        private int minimum = 1;
        /// <summary>
        /// When this argument is a collection, gets or sets the minimal number of values it may get; otherwise ignored.
        /// </summary>
        public int Minimum
        {
            get => minimum;
            set
            {
                if (value > Maximum)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        $"The {nameof(Minimum)} value must not be greater than the {nameof(Maximum)} value");
                else minimum = value;
            }
        }

        private int maximum = int.MaxValue;
        /// <summary>
        /// When this argument is a collection, gets or sets the maximal number of values it may get; otherwise ignored.
        /// </summary>
        public int Maximum
        {
            get => maximum;
            set
            {
                if (value < Minimum)
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        $"The {nameof(Maximum)} value must not be lower than the {nameof(Minimum)} value");
                else maximum = value;
            }
        }
    }
}
