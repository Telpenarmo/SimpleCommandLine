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
        private readonly IFormatProvider formatProvider;

        public TokensParserFactory(IEnumerable<ParsingTypeInfo> registeredTypes, IConvertersFactory convertersFactory, IFormatProvider formatProvider)
        {
            this.registeredTypes = registeredTypes ?? throw new ArgumentNullException(nameof(registeredTypes));
            this.convertersFactory = convertersFactory ?? throw new ArgumentNullException(nameof(convertersFactory));
            this.formatProvider = formatProvider ?? throw new ArgumentNullException(nameof(formatProvider));
        }

        public ITokensParser Build()
        {
            var typeInfo = registeredTypes.SingleOrDefault(x => !(x is ParsingCommandTypeInfo))
                ?? throw new InvalidOperationException("No generic types were declared.");
            return CreateParser(typeInfo);
        }

        public ITokensParser Build(string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
                return Build();
            var typeInfo = registeredTypes.OfType<ParsingCommandTypeInfo>().SingleOrDefault(x => x.Aliases.Contains(commandName))
                ?? throw new InvalidOperationException("This command was not declared.");
            return CreateParser(typeInfo);
        }

        private TokensParser CreateParser(ParsingTypeInfo typeInfo)
            => new TokensParser(() => new ObjectBuilder(typeInfo, convertersFactory, formatProvider));
    }
}