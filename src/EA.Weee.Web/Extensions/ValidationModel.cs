namespace EA.Weee.Web.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;

    public static class ValidationModel
    {
        public static bool ValidateModel(object model, ModelStateDictionary modelState, string prefix = "")
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            bool isValid = true;

            if (model is IEnumerable enumerable && !(model is string))
            {
                int index = 0;
                foreach (var item in enumerable)
                {
                    string itemPrefix = string.IsNullOrEmpty(prefix) ? $"[{index}]" : $"{prefix}[{index}]";
                    isValid &= ValidateObject(item, modelState, itemPrefix);
                    index++;
                }
            }
            else
            {
                isValid = ValidateObject(model, modelState, prefix);
            }

            return isValid;
        }

        private static bool ValidateObject(object obj, ModelStateDictionary modelState, string prefix)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(obj, serviceProvider: null, items: null);
            bool isValid =
                Validator.TryValidateObject(obj, validationContext, validationResults, validateAllProperties: true);

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    var fullMemberName = string.IsNullOrEmpty(prefix) ? memberName : $"{prefix}.{memberName}";

                    if (!modelState.ContainsKey(fullMemberName) ||
                        !modelState[fullMemberName].Errors.Any(e => e.ErrorMessage == validationResult.ErrorMessage))
                    {
                        modelState.AddModelError(fullMemberName, validationResult.ErrorMessage);
                    }
                }
            }

            return isValid;
        }
    }
}