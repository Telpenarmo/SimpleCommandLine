namespace SimpleCommandLine.Registration.Validation
{
    internal interface IPropertyValidator
    {
        bool Verify (System.Reflection.PropertyInfo propertyInfo);
    }
}