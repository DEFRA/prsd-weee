namespace EA.Prsd.Core.Web.Mvc.Extensions
{
    using System.Web.Mvc;
    using ApiClient;

    public static class ControllerExtensions
    {
        public static void AddValidationErrorsToModelState(this Controller controller, ApiResponse apiResponse)
        {
            foreach (var error in apiResponse.Errors)
            {
                controller.ModelState.AddModelError(string.Empty, error);
            }
        }
    }
}