using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleCommandLine.Tests
{
    public static class TestData
    {
        public static IEnumerable<object[]> GetNullAndEmptyStrings()
        {
            yield return new[] { "" };
            yield return new[] { string.Empty };
            yield return new string[] { null };
        }
    }
}
