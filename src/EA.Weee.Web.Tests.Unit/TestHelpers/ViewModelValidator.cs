namespace EA.Weee.Web.Tests.Unit.TestHelpers
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public static class ViewModelValidator
    {
        /// <summary>
        /// Checks the ViewModel according to the logic used by the MVC Controller ModelState.IsValid check.
        /// </summary>
        /// <param name="viewModel">The ViewModel object to check.</param>
        /// <returns>A list of validation results.</returns>
        public static List<ValidationResult> ValidateViewModel(object viewModel)
        {
            var validationContext = new ValidationContext(viewModel);

            var validationResults = new List<ValidationResult>();

            Validator.TryValidateObject(viewModel, validationContext, validationResults, true);

            return validationResults;
        }
    }
}
