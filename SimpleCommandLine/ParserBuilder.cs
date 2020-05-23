using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Parsing.Converters;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Registration.Validation;
using SimpleCommandLine.Tokenization.Tokenizers;
using SimpleCommandLine.Tokenization.Tokenizers.POSIX;

namespace SimpleCommandLine
{
    /// <summary>
    /// Configures and constructs <see cref="Parser"/> instance.
    /// </summary>
    public class ParserBuilder
    {
        private readonly List<ParsingTypeInfo> types;
        private readonly TypeRegisterer typeRegisterer;
        private readonly IConvertersFactory convertersFactory = new ConvertersFactory();
        private readonly FinalValidator finalVerifier = new FinalValidator();
        private readonly TypeValidator typeValidator;
        private readonly PropertyValidator argumentValidator = new PropertyValidator();
        private ITokenizerBuilder tokenizerBuilder;
        private IFormatProvider formatProvider;
                
        public ParserBuilder()
        {
            RegisterTokenization<POSIXTokenizerBuilder>(x => x.AllowShortOptionGroups = true);
            types = new List<ParsingTypeInfo>();
            typeValidator = new TypeValidator(types);
            typeRegisterer = new TypeRegisterer(typeValidator, argumentValidator);
            LoadDefaultConverters();
        }

        /// <summary>
        /// Constructs <see cref="Parser"/> instance with current settings.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when registration was invalid.</exception>
        public Parser Build()
        {
            finalVerifier.Verify(types, convertersFactory);
            return new Parser(PrepareTokenizer(), PrepareTypeParserFactory());
        }

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
            types.Add(typeRegisterer.Register(factory));
        }

        /// <summary>
        /// Registers a custom convention, eg. POSIX- or PowerShell-like.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="ITokenizerBuilder"/> to be constructed.</typeparam>
        /// <param name="tokenizationBuilder">Action configuring the given builder.</param>
        public void RegisterTokenization<T>(Action<T> tokenizationBuilder) where T : ITokenizerBuilder, new ()
        {
            var t = new T();
            tokenizationBuilder(t);
            tokenizerBuilder = t;
        }

        /// <summary>
        /// Registers converter of specifed type.
        /// </summary>
        /// <typeparam name="T">Type to which string values are converted.</typeparam>
        /// <param name="converter">Object that will convert string values to <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="converter"/> is null.</exception>
        public void RegisterConverter<T>(IValueConverter<T> converter)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
            convertersFactory.RegisterConverter(converter, typeof(T));
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
            var commandsNames = types.OfType<ParsingCommandTypeInfo>().SelectMany(x => x.Aliases);
            var tokenizer = tokenizerBuilder.BuildTokenizer();
            return new CommandTokenizer(commandsNames) { Next = tokenizer };
        }

        private TokensParserFactory PrepareTypeParserFactory()
            => new TokensParserFactory(types, convertersFactory, formatProvider);

        private void LoadDefaultConverters()
        {
            RegisterConverter(StockConverters.StringConverter);
            RegisterConverter(new BoolValueConverter());
            RegisterConverter(NumericValueConverters.ByteConverter);
            RegisterConverter(NumericValueConverters.DoubleConverter);
            RegisterConverter(NumericValueConverters.FloatConverter);
            RegisterConverter(NumericValueConverters.DecimalConverter);
            RegisterConverter(NumericValueConverters.Int16Converter);
            RegisterConverter(NumericValueConverters.Int32Converter);
            RegisterConverter(NumericValueConverters.Int64Converter);
            RegisterConverter(StockConverters.DateTimeConverter);
            RegisterConverter(StockConverters.TimeSpanConverter);
        }
    }
}