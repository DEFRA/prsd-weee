namespace EA.Weee.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public static class ValidationModel
    {
        public static bool ValidateModel(object model, ModelStateDictionary modelState, string prefix = "")
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, serviceProvider: null, items: null);
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    // If a prefix is provided, concatenate it with the member name using dot notation
                    var fullMemberName = string.IsNullOrEmpty(prefix) ? memberName : $"{prefix}.{memberName}";
                    // Add the error to ModelState using the fully qualified member name
                    modelState.AddModelError(fullMemberName, validationResult.ErrorMessage);
                }
            }

            return isValid;
        }
    }
}