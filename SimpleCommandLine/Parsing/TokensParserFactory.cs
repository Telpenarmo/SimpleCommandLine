using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;

namespace SimpleCommandLine.Parsing
{
    internal class TokensParserFactory : ITokensParserFactory
    {
        private readonly IEnumerable<ParsingTypeInfo> registeredTypes;
        private readonly IConvertersFactory convertersFactory;
        private TokensParser cachedGenericParser;
        private readonly Dictionary<string, TokensParser> cachedVerbParsers = new Dictionary<string, TokensParser>();
        private readonly IFormatProvider formatProvider;

        public TokensParserFactory(IEnumerable<ParsingTypeInfo> registeredTypes, IConvertersFactory convertersFactory, IFormatProvider formatProvider)
        {
            this.registeredTypes = registeredTypes ?? throw new ArgumentNullException(nameof(registeredTypes));
            this.convertersFactory = convertersFactory ?? throw new ArgumentNullException(nameof(convertersFactory));
            this.formatProvider = formatProvider ?? throw new ArgumentNullException(nameof(formatProvider));
        }

        public ITokensParser Build()
        {
            if (cachedGenericParser is null)
            {
                var typeInfo = registeredTypes.SingleOrDefault(x => !(x is ParsingCommandTypeInfo))
                    ?? throw new InvalidOperationException("No generic types were declared.");
                cachedGenericParser = new TokensParser(new ObjectBuilder(typeInfo, convertersFactory, formatProvider));
            }
            return cachedGenericParser;
        }

        public ITokensParser Build(string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
                return Build();

            if (!cachedVerbParsers.ContainsKey(commandName))
            {
                var typeInfo = registeredTypes.OfType<ParsingCommandTypeInfo>().SingleOrDefault(x => x.Aliases.Contains(commandName))
                      ?? throw new InvalidOperationException("This command was not declared.");
                CacheTypeParser(new TokensParser(new ObjectBuilder(typeInfo, convertersFactory, formatProvider)), typeInfo.Aliases);
            }

            return cachedVerbParsers[commandName];
        }

        private void CacheTypeParser(TokensParser typeParser, IEnumerable<string> names)
        {
            foreach (var name in names)
                cachedVerbParsers.Add(name, typeParser);
        }
    }
}