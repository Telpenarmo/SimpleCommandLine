using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Registration;

namespace SimpleCommandLine.Parsing
{
    internal interface IResultBuilderFactory
    {
        ResultBuilder? Build();
        ResultBuilder Build(string commandName);
    }

    internal class ResultBuilderFactory : IResultBuilderFactory
    {
        private readonly IEnumerable<TypeInfo> registeredTypes;
        private readonly ConvertersFactory convertersFactory;
        private readonly IFormatProvider formatProvider;

        public ResultBuilderFactory(IEnumerable<TypeInfo> registeredTypes, ConvertersFactory convertersFactory, IFormatProvider formatProvider)
        {
            this.registeredTypes = registeredTypes;
            this.convertersFactory = convertersFactory;
            this.formatProvider = formatProvider;
        }

        public ResultBuilder? Build()
        {
            var typeInfo = registeredTypes.SingleOrDefault(t => !t.Aliases.Any());
            return typeInfo == null ? null : Create(typeInfo);
        }

        public ResultBuilder Build(string commandName)
        {
            var typeInfo = registeredTypes.SingleOrDefault(t => t.Aliases.Contains(commandName))
                ?? throw new InvalidOperationException("This command was not defined.");
            return Create(typeInfo);
        }

        private ResultBuilder Create(TypeInfo typeInfo)
            => new ResultBuilder(typeInfo, convertersFactory, formatProvider);
    }
}