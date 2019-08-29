namespace EA.Prsd.Core.Validation
{
    using System.ComponentModel.DataAnnotations;

    public class MustBeTrueAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return value is bool && (bool)value;
        }
    }
}