namespace EA.Weee.Web.Extensions
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public static class ModelStateExtensions
    {
        public static void RunCustomValidation(this ModelStateDictionary modelState, IValidatableObject viewModel)
        {
            var errors = viewModel.Validate(new ValidationContext(viewModel, null, null));
            foreach (var error in errors)
            {
                foreach (var memberName in error.MemberNames)
                {
                    modelState.AddModelError(memberName, error.ErrorMessage);
                }
            }
        }

        public static void ApplyCustomValidationSummaryOrdering(this ModelStateDictionary modelState, IEnumerable<string> modelStateKeys)
        {
            var result = new ModelStateDictionary();
            foreach (string key in modelStateKeys)
            {
                if (modelState.ContainsKey(key) && !result.ContainsKey(key))
                {
                    result.Add(key, modelState[key]);
                }
            }

            foreach (string key in modelState.Keys)
            {
                if (!result.ContainsKey(key))
                {
                    result.Add(key, modelState[key]);
                }
            }

            modelState.Clear();
            modelState.Merge(result);
        }
    }
}