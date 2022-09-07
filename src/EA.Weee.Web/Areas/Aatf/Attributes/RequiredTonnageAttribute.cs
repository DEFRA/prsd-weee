﻿namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core;
    using ViewModels;
    using Web.ViewModels.Shared;

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredTonnageAttribute : RequiredTonnageBaseAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as IActionModel;

            Guard.ArgumentNotNull(() => model, model, "RequiredTonnageAttribute Model is null");
            Guard.ArgumentNotNull(() => value, value, "RequiredTonnageAttribute Tonnage Values are null");

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