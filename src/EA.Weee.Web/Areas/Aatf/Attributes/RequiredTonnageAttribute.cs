namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.Aatf;
    using Core.AatfEvidence;
    using Prsd.Core;
    using ViewModels;
    using Web.ViewModels.Shared;

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredTonnageAttribute : RequiredAttribute
    {
        private const string Message = "Enter a tonnage value for at least one category. The value must be 3 decimal places or less.";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as IActionModel;

            Guard.ArgumentNotNull(() => model, model, "RequiredTonnageAttribute Model is null");
            Guard.ArgumentNotNull(() => value, value, "RequiredTonnageAttribute Tonnage Values are null");

            var list = value as IList<EvidenceCategoryValue>;

            if (model.Action.Equals(ActionEnum.Submit))
            {
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
            }

            return ValidationResult.Success;
        }
    }
}