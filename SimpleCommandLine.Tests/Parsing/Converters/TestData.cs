using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleCommandLine.Tests.Parsing.Converters
{
    public static class TestData
    {
        public static IEnumerable<object[]> GetAllowedNumbersStrings(bool signed, bool allowThousands = true, bool allowExponent = true)
        {
            yield return new object[] { "1", 1 };
            yield return new object[] { "+1", 1 };
            yield return new object[] { "1+", 1 };
            yield return new object[] { "¤1", 1 };
            yield return new object[] { "1¤", 1 };
            yield return new object[] { "   1   ", 1 };
            yield return new object[] { "1.0", 1 };

            if (signed)
            {
                yield return new object[] { "-1", -1 };
                yield return new object[] { "1-", -1 };
                yield return new object[] { "-1.0", -1 };
            }

            if (allowThousands)
            {
                yield return new object[] { "123,000,000", 123000000 };
            }

            if (allowExponent)
            {
                yield return new object[] { "1e1", 10 };
                yield return new object[] { "1E1", 10 };
                yield return new object[] { "1.23e2", 123 };
            }
        }

        public static IEnumerable<object[]> GetNullAndEmptyStrings() => Tests.TestData.GetNullAndEmptyStrings();
    }
}
