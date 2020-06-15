using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;

namespace SimpleCommandLine.Parsing
{
    internal class ObjectBuilderFactory
    {
        private readonly IEnumerable<ParsingTypeInfo> registeredTypes;
        private readonly IConvertersFactory convertersFactory;
        private readonly IFormatProvider formatProvider;

        public ObjectBuilderFactory(IEnumerable<ParsingTypeInfo> registeredTypes, IConvertersFactory convertersFactory, IFormatProvider formatProvider)
        {
            this.registeredTypes = registeredTypes;
            this.convertersFactory = convertersFactory;
            this.formatProvider = formatProvider;
        }

        public ObjectBuilder Build()
        {
            var typeInfo = registeredTypes.SingleOrDefault(x => x.Name == string.Empty);
            return typeInfo == null ? null : Create(typeInfo);
        }

        public ObjectBuilder Build(string commandName)
        {
            var typeInfo = registeredTypes.SingleOrDefault(x => x.Name == commandName)
                ?? throw new InvalidOperationException("This command was not defined.");
            return Create(typeInfo);
        }

        private ObjectBuilder Create(ParsingTypeInfo typeInfo)
            => new ObjectBuilder(typeInfo, convertersFactory, formatProvider);
    }
}