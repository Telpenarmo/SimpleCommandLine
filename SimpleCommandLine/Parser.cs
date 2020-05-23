using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Tokenization.Tokens;
using SimpleCommandLine.Tokenization.Tokenizers;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("SimpleCommandLine.Tests")]

namespace SimpleCommandLine
{
    /// <summary>
    /// Used to parse command-line arguments.
    /// </summary>
    public class Parser
    {
        private readonly TokensParserFactory typeParserFactory;
        private IArgumentTokenizer tokenizer;
        private readonly List<object> results = new List<object>();

        internal Parser(IArgumentTokenizer tokenizer, TokensParserFactory typeParserFactory)
        {
            this.tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
            this.typeParserFactory = typeParserFactory ?? throw new ArgumentNullException(nameof(typeParserFactory));
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
            IEnumerable<IArgumentToken> tokens = Tokenize(args ?? Enumerable.Empty<string>()).ToArray();

            IEnumerable<IArgumentToken> generic = tokens.TakeWhile(t => !(t is CommandToken));
            int count = generic.Count();
            if (count > 0)
                results.Add(typeParserFactory.Build().Parse(generic));
            if (count < tokens.Count())
            {
                var commandToken = tokens.First(t => t is CommandToken) as CommandToken;
                var resultType = typeParserFactory.Build(commandToken.Alias).Parse(tokens.Skip(count + 1));
                results.Add(resultType);
            }
            
            return new Result(results);
        }

        private IEnumerable<IArgumentToken> Tokenize(IEnumerable<string> args)
        {
            ChainTokenizer commandTokenizer = tokenizer as ChainTokenizer;
            foreach (var str in args)
            {
                var token = tokenizer.TokenizeArgument(str);
                if (token is CommandToken)
                {
                    tokenizer = commandTokenizer.Next;
                }
                yield return token;
            }
            tokenizer = commandTokenizer;
        }
    }
}