﻿using System;
using System.Collections.Generic;
using System.Linq;
using SimpleCommandLine.Tokenization.Tokens;

namespace SimpleCommandLine.Tokenization.Tokenizers
{
    /// <summary>
    /// Builds the <see cref="AssignedValueToken"/>.
    /// </summary>
    public class CommandTokenizer : ChainTokenizer
    {
        private readonly IEnumerable<string> commandNames;

        public CommandTokenizer(IEnumerable<string> commandNames)
        {
            this.commandNames = commandNames ?? throw new ArgumentNullException(nameof(commandNames));
        }

        /// <summary>
        /// Checks if this tokenizer is able to handle given argument.
        /// </summary>
        /// <param name="arg">An argument to check.</param>

        public override bool CanHandle(string arg) => !string.IsNullOrWhiteSpace(arg) && commandNames.Contains(arg);

        /// <summary>
        /// Tokenizes given argument assuming its correctness.
        /// </summary>
        /// <param name="arg">An argument checked by <see cref="CanHandle(string)"/> method.</param>
        public override IArgumentToken Handle(string arg) => new CommandToken(arg);
    }
}