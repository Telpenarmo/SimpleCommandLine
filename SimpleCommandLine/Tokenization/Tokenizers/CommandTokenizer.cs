﻿using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers
{
    public class CommandTokenizer : ChainTokenizer
    {
        private readonly IEnumerable<string> commandNames;

        public CommandTokenizer(IEnumerable<string> commandNames)
        {
            this.commandNames = commandNames ?? throw new ArgumentNullException(nameof(commandNames));
        }

        public override bool CanHandle(string arg) => string.IsNullOrWhiteSpace(arg) ? false : commandNames.Contains(arg);

        public override IArgumentToken Handle(string arg) => new CommandToken(arg);
    }
}