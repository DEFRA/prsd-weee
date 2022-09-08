namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System.Collections;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.AatfEvidence;

    public abstract class RequiredTonnageBaseAttribute : RequiredAttribute
    {
        private const string Message = "Enter a tonnage value for at least one category. The value must be 3 decimal places or less.";

        protected ValidationResult ValidateTonnage(object value)
        {
            var list = (value as IList).Cast<IEvidenceCategoryValue>();

            if (list == null || !list.Any())
            {
                return new ValidationResult(Message);
            }

            var anyValidValues = list.Where(v => v.Received != null)
                .Where(val => decimal.TryParse(val.Received, out var converted) && converted > 0);

            if (!anyValidValues.Any())
            {
                return new ValidationResult(Message);
            }

            return ValidationResult.Success;
        }
    }
}