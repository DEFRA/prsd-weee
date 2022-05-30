namespace EA.Weee.Core.Validation
{
    public interface ITonnageValueValidator
    {
        TonnageValidationResult Validate(object value);
    }
}
