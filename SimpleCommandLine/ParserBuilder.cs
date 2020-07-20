using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Parsing.Converters;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenizers;
using SimpleCommandLine.Tokenizers.POSIX;

namespace SimpleCommandLine
{
    /// <summary>
    /// Configures and constructs <see cref="Parser"/> instance.
    /// </summary>
    public class ParserBuilder
    {
        private readonly List<TypeInfo> types = new List<TypeInfo>();
        private readonly TypeRegisterer typeRegisterer;
        private readonly ConvertersFactory convertersFactory;
        private IFormatProvider formatProvider;
        private bool globalTypeSet;


        public ParserBuilder()
        {
            convertersFactory = new ConvertersFactory(Settings);
            LoadDefaultConverters();
            typeRegisterer = new TypeRegisterer(convertersFactory);
        }

        /// <summary>
        /// Constructs <see cref="Parser"/> instance with current settings.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when registration was invalid.</exception>
        public Parser Build()
        {
            TokenizerBuilder ??= new POSIXTokenizerBuilder() { AllowShortOptionGroups = true };
            return new Parser(PrepareTokenizer(), PrepareObjectBuilderFactory());
        }

        /// <summary>
        /// Gets the parsing settings.
        /// </summary>
        /// <returns></returns>
        public ParsingSettings Settings { get; } = new ParsingSettings();

        /// <summary>
        /// Gets or sets an <see cref="ITokenizerBuilder"/> that will be used to
        /// construct a tokenizer following a tokenization convention, by default POSIX-like.
        /// </summary>
        public ITokenizerBuilder TokenizerBuilder { get; set; }

        /// <summary>
        /// Registers a type that has a parameterless constructor.
        /// </summary>
        /// <typeparam name="T">Type to be registered.</typeparam>
        /// <exception cref="InvalidOperationException">Thrown when the type to be registered is invalid.</exception>
        public void RegisterType<T>() where T : new()
            => RegisterType(() => new T());

        /// <summary>
        /// Registers a type with a specified factory method.
        /// </summary>
        /// <typeparam name="T">Type to be registered.</typeparam>
        /// <param name="factory">Returns type to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="factory"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the type to be registered is invalid.</exception>
        public void RegisterType<T>(Func<T> factory)
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            var type = typeRegisterer.Register(factory);

            if (type.Name == string.Empty)
            {
                if (globalTypeSet)
                    throw new InvalidOperationException("You can register only one non-command type");
                globalTypeSet = true;
            }

            types.Add(type);
        }

        /// <summary>
        /// Registers converter of specifed type.
        /// </summary>
        /// <typeparam name="T">Type to which string values are converted.</typeparam>
        /// <param name="converter">Object that will convert string values to <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="converter"/> is null.</exception>
        public void RegisterConverter(ISingleValueConverter converter, Type type)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            convertersFactory.RegisterConverter(converter, type);
        }

        /// <summary>
        /// Registers a custom <see cref="IFormatProvider"/> that is being used in conversion.
        /// </summary>
        /// <param name="formatProvider">A custom <see cref="IFormatProvider"/> to be used.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="formatProvider"/> is null.</exception>
        public void UseFormatProvider(IFormatProvider formatProvider)
        {
            this.formatProvider = formatProvider ?? throw new ArgumentNullException(nameof(formatProvider));
        }

        private ChainTokenizer PrepareTokenizer()
        {
            var commandsNames = types.Select(x => x.Name);
            var tokenizer = TokenizerBuilder.BuildTokenizer();
            return new CommandTokenizer(commandsNames) { Next = tokenizer };
        }

        private ObjectBuilderFactory PrepareObjectBuilderFactory()
            => new ObjectBuilderFactory(types, convertersFactory, formatProvider);

        private void LoadDefaultConverters()
        {
            RegisterConverter(StockConverters.StringConverter, typeof(string));
            RegisterConverter(StockConverters.StringConverter, typeof(object));
            RegisterConverter(new BoolValueConverter(), typeof(bool));
            RegisterConverter(NumericalValueConverters.ByteConverter, typeof(byte));
            RegisterConverter(NumericalValueConverters.SByteConverter, typeof(sbyte));
            RegisterConverter(NumericalValueConverters.DoubleConverter, typeof(double));
            RegisterConverter(NumericalValueConverters.FloatConverter, typeof(float));
            RegisterConverter(NumericalValueConverters.DecimalConverter, typeof(decimal));
            RegisterConverter(NumericalValueConverters.Int16Converter, typeof(short));
            RegisterConverter(NumericalValueConverters.Int32Converter, typeof(int));
            RegisterConverter(NumericalValueConverters.Int64Converter, typeof(long));
            RegisterConverter(NumericalValueConverters.UInt16Converter, typeof(ushort));
            RegisterConverter(NumericalValueConverters.UInt32Converter, typeof(uint));
            RegisterConverter(NumericalValueConverters.UInt64Converter, typeof(ulong));
            RegisterConverter(StockConverters.GuidConverter, typeof(Guid));
            RegisterConverter(StockConverters.IPAdressConverter, typeof(System.Net.IPAddress));
            RegisterConverter(StockConverters.DateTimeConverter, typeof(DateTime));
            RegisterConverter(StockConverters.DateTimeOffsetConverter, typeof(DateTimeOffset));
            RegisterConverter(StockConverters.TimeSpanConverter, typeof(TimeSpan));
            RegisterConverter(StockConverters.URIConverter, typeof(Uri));
        }
    }
}