namespace EA.Prsd.Core.Web.Mvc.Extensions
{
    using System.Web.Mvc;
    using ApiClient;

    public static class ControllerExtensions
    {
        public static void HandleBadRequest(this Controller controller, ApiBadRequestException apiBadRequestException)
        {
            if (apiBadRequestException.BadRequestData.ModelState != null)
            {
                foreach (var modelStateItem in apiBadRequestException.BadRequestData.ModelState)
                {
                    foreach (var message in modelStateItem.Value)
                    {
                        controller.ModelState.AddModelError(modelStateItem.Key, message);
                    }
                }
            }

            if (string.Equals(apiBadRequestException.BadRequestData.Error, "invalid_grant"))
            {
                controller.ModelState.AddModelError(string.Empty, apiBadRequestException.BadRequestData.ErrorDescription);
            }
        }
    }
}