namespace SimpleCommandLine
{
    public struct ParsingSettings
    {
        public ParsingSettings(bool ignoreCaseOnEnumConversion, bool acceptNumericalEnumValues)
        {
            IgnoreCaseOnEnumConversion = ignoreCaseOnEnumConversion;
            AcceptNumericalEnumValues = acceptNumericalEnumValues;
        }

        public bool IgnoreCaseOnEnumConversion { get; }
        public bool AcceptNumericalEnumValues { get; }
    }
}