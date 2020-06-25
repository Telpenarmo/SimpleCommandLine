using System;
using SimpleCommandLine;

namespace Demo
{
    [Command("add")]
    class AddCommand<T>
    {
        [Value(0)]
        public T Item { get; set; }
    }
}