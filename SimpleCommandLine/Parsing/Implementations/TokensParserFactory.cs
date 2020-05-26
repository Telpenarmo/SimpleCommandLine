using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;

namespace SimpleCommandLine.Parsing
{
    internal class TokensParserFactory
    {
        private readonly IEnumerable<ParsingTypeInfo> registeredTypes;
        private readonly IConvertersFactory convertersFactory;
        private readonly IFormatProvider formatProvider;

        public TokensParserFactory(IEnumerable<ParsingTypeInfo> registeredTypes, IConvertersFactory convertersFactory, IFormatProvider formatProvider)
        {
            this.registeredTypes = registeredTypes;
            this.convertersFactory = convertersFactory;
            this.formatProvider = formatProvider;
        }

        public TokensParser Build()
        {
            var typeInfo = registeredTypes.SingleOrDefault(x => !(x is ParsingCommandTypeInfo))
                ?? throw new InvalidOperationException("The global type was not defined.");
            return CreateParser(typeInfo);
        }

        public TokensParser Build(string commandName)
        {
            var typeInfo = registeredTypes.OfType<ParsingCommandTypeInfo>().SingleOrDefault(x => x.Name == commandName)
                ?? throw new InvalidOperationException("This command was not defined.");
            return CreateParser(typeInfo);
        }

        private TokensParser CreateParser(ParsingTypeInfo typeInfo)
            => new TokensParser(() => new ObjectBuilder(typeInfo, convertersFactory, formatProvider));
    }
}