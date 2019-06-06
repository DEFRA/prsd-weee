namespace EA.Weee.Web.Extensions
{
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
    }
}