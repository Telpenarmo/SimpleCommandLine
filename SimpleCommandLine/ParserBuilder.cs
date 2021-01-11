﻿using System;
using System.Collections.Generic;
using System.Globalization;
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
        private readonly List<Func<TypeInfo>> typeFactories = new List<Func<TypeInfo>>();
        private readonly ConvertersFactory convertersFactory = new ConvertersFactory();
        private readonly TypeRegisterer typeRegisterer;


        public ParserBuilder()
        {
            typeRegisterer = new TypeRegisterer(convertersFactory);
            LoadDefaultConverters();
        }

        /// <summary>
        /// Constructs <see cref="Parser"/> instance with current settings.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when registration was invalid.</exception>
        public Parser Build()
        {
            convertersFactory.Settings = Settings;

            var types = new List<TypeInfo>();
            bool globalTypeSet = false;
            foreach (var typeFactory in typeFactories)
            {
                var type = typeFactory();
                if (type.Aliases.Any())
                    types.Add(type);
                else if (globalTypeSet)
                    throw new InvalidOperationException("You can register only one non-command type");
                else globalTypeSet = true;
            }
            TokenizerBuilder ??= new POSIXTokenizerBuilder() { AllowShortOptionGroups = true };

            var commandsNames = types.SelectMany(x => x.Aliases);
            var tokenizer = TokenizerBuilder.BuildTokenizer();
            tokenizer = new CommandTokenizer(commandsNames) { Next = tokenizer };
            var objectBuilderFactory = new ResultBuilderFactory(types, convertersFactory, FormatProvider);
            return new Parser(tokenizer, objectBuilderFactory);
        }

        /// <summary>
        /// Gets or sets the parsing settings.
        /// </summary>
        public ParsingSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the <seealso cref="IFormatProvider" /> that will be used in parsing proccess.
        /// </summary>
        public IFormatProvider FormatProvider { get; set; } = CultureInfo.CurrentUICulture;

        /// <summary>
        /// Gets or sets an <see cref="ITokenizerBuilder"/> that will be used to
        /// construct a tokenizer following a tokenization convention, by default POSIX-like.
        /// </summary>
        public ITokenizerBuilder? TokenizerBuilder { get; set; }

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

            typeFactories.Add(() => typeRegisterer.Register(factory));
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

        public List<string> TrueAliases { get; set; } = new List<string> { "true" };
        public List<string> FalseAliases { get; set; } = new List<string> { "false" };

        private void LoadDefaultConverters()
        {
            RegisterConverter(StockConverters.StringConverter, typeof(string));
            RegisterConverter(StockConverters.StringConverter, typeof(object));
            RegisterConverter(new BoolValueConverter(TrueAliases.ToArray(), FalseAliases.ToArray()), typeof(bool));
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