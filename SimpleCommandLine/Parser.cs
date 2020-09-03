using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tokens;
using SimpleCommandLine.Tokenizers;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SimpleCommandLine.Tests")]
namespace SimpleCommandLine
{
    /// <summary>
    /// Used to parse command-line arguments.
    /// </summary>
    public class Parser
    {
        private readonly IResultBuilderFactory objectBuilderFactory;
        
        private readonly IArgumentTokenizer tokenizer;

        internal Parser(IArgumentTokenizer tokenizer, IResultBuilderFactory objectBuilderFactory)
        {
            this.tokenizer = tokenizer;
            this.objectBuilderFactory = objectBuilderFactory;
        }

        /// <summary>
        /// Parses a collection of arguments.
        /// </summary>
        /// <param name="args">Collection of command-line arguments to be parsed.</param>
        /// <returns>Instance of the <see cref="Result"/> class that is used to consume the parsing result.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public Result Parse(IEnumerable<string> args)
        {
            var parser = new TokensParser(objectBuilderFactory);
            var tokens = (args ?? Enumerable.Empty<string>())
                .Select(arg => tokenizer.TokenizeArgument(arg));
            return parser.Parse(tokens);
        }
    }
}