using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Parsing;
using SimpleCommandLine.Parsing.Converters;
using SimpleCommandLine.Registration;
using SimpleCommandLine.Tokenization.Tokenizers;
using SimpleCommandLine.Tokenization.Tokenizers.POSIX;

namespace SimpleCommandLine
{
    /// <summary>
    /// Configures and constructs <see cref="Parser"/> instance.
    /// </summary>
    public class ParserBuilder
    {
        private readonly List<ParsingTypeInfo> types = new List<ParsingTypeInfo>();
        private readonly TypeRegisterer typeRegisterer;
        private readonly ConvertersFactory convertersFactory = new ConvertersFactory();
        private ITokenizerBuilder tokenizerBuilder;
        private IFormatProvider formatProvider;
        private bool globalTypeSet;

        public ParserBuilder()
        {
            LoadDefaultConverters();
            RegisterTokenization(new POSIXTokenizerBuilder());
            typeRegisterer = new TypeRegisterer(convertersFactory);
        }

        /// <summary>
        /// Constructs <see cref="Parser"/> instance with current settings.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when registration was invalid.</exception>
        public Parser Build()
        {
            return new Parser(PrepareTokenizer(), PrepareObjectBuilderFactory());
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
        /// Registers a custom convention, eg. POSIX- or PowerShell-like.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="ITokenizerBuilder"/> to be constructed.</typeparam>
        /// <param name="tokenizationBuilder">Action configuring the given builder.</param>
        public void RegisterTokenization(ITokenizerBuilder tokenizerBuilder)
        {
            this.tokenizerBuilder = tokenizerBuilder;
        }

        /// <summary>
        /// Registers converter of specifed type.
        /// </summary>
        /// <typeparam name="T">Type to which string values are converted.</typeparam>
        /// <param name="converter">Object that will convert string values to <typeparamref name="T"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="converter"/> is null.</exception>
        public void RegisterConverter(IValueConverter converter, Type type)
        {
            if (converter == null)
                throw new ArgumentNullException(nameof(converter));
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
            var tokenizer = tokenizerBuilder.BuildTokenizer();
            return new CommandTokenizer(commandsNames) { Next = tokenizer };
        }

        private ObjectBuilderFactory PrepareObjectBuilderFactory()
            => new ObjectBuilderFactory(types, convertersFactory, formatProvider);

        private void LoadDefaultConverters()
        {
            RegisterConverter(new StringValueConverter(), typeof(string));
            RegisterConverter(new BoolValueConverter(), typeof(bool));
            RegisterConverter(new FloatValueConverter(), typeof(float));
            RegisterConverter(new IntValueConverter(), typeof(int));
        }
    }
}