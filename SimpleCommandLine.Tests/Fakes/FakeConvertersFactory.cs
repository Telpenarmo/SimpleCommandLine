using System;
using System.Collections.Generic;
using SimpleCommandLine.Parsing;

namespace SimpleCommandLine.Tests.Fakes
{
    public class FakeConvertersFactory : IConvertersFactory
    {
        private readonly Dictionary<Type, ISingleValueConverter> savedConverters = new();
        public IConverter this[Type type] => savedConverters[type];

        public ParsingSettings Settings { get; set; }

        public bool CheckForType(Type type) => savedConverters.ContainsKey(type);

        public void RegisterConverter(ISingleValueConverter converter, Type type) => savedConverters.Add(type, converter);
    }
}