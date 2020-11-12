using System.Collections.Generic;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Parsing.Converters;
using SimpleCommandLine.Registration;

namespace SimpleCommandLine.Tests.Fakes
{
    internal class FakeResultBuilderFactory : IResultBuilderFactory
    {
        public ResultBuilder Build()
        {
            var convertersFactory = new ConvertersFactory();
            convertersFactory.RegisterConverter(StockConverters.StringConverter, typeof(string));
            convertersFactory.RegisterConverter(NumericalValueConverters.Int32Converter, typeof(int));
            convertersFactory.RegisterConverter(new BoolValueConverter(), typeof(bool));
            return new ResultBuilder(
                new TypeInfo(new ParameterInfo[] { }, new Dictionary<string, ParameterInfo>(), TestObject.Factory),
                new ConvertersFactory(), System.Globalization.CultureInfo.InvariantCulture);
        }

        public ResultBuilder Build(string commandName)
        {
            throw new System.NotImplementedException();
        }
    }
}