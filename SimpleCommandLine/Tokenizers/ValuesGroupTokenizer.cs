using System;
using System.Linq;
using SimpleCommandLine.Tokens;

namespace SimpleCommandLine.Tokenizers
{
    /// <summary>
    /// Builds the <see cref="ValuesGroupToken"/>.
    /// </summary>
    public class ValuesGroupTokenizer : ChainTokenizer
    {
        private readonly char[] separators;
        private readonly ValueTokenizer valueTokenizer;

        public ValuesGroupTokenizer(char[] separators, ValueTokenizer valueTokenizer)
        {
            this.valueTokenizer = valueTokenizer ?? throw new ArgumentNullException(nameof(valueTokenizer));
            if (separators.Length == 0)
                throw new ArgumentException("At least one separator must be defined.");
            this.separators = separators;
        }

        /// <summary>
        /// Checks if this tokenizer is able to handle given argument.
        /// </summary>
        /// <param name="arg">An argument to check.</param>
        public override bool CanHandle(string arg) => separators.Any(s => arg.Contains(s));

        /// <summary>
        /// Tokenizes given argument assuming its correctness.
        /// </summary>
        /// <param name="arg">An argument checked by <see cref="CanHandle(string)"/> method.</param>
        public override IArgumentToken Handle(string arg)
        {
            int sepIndex = -1;
            for (int i = 0; i < separators.Length; i++)
            {
                if (arg.IndexOf(separators[i]) == -1) continue;
                sepIndex = i;
                break;
            }
            return HandleRecursively(arg, sepIndex);
        }

        private ValueToken HandleRecursively(string arg, int sepIndex)
        {
            var splitted = arg.Split(separators[sepIndex]);

            var tokens = separators.Length == sepIndex
                ? splitted.Select(arg => valueTokenizer.TokenizeArgument(arg) as ValueToken)
                : splitted.Select(arg => HandleRecursively(arg, sepIndex + 1));

            return splitted.Length == 1 ? tokens.Single() : new ValuesGroupToken(tokens, arg);
        }
    }
}