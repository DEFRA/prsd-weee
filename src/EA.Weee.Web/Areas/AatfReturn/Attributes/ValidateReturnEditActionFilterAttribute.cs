namespace EA.Weee.Web.Areas.AatfReturn.Attributes
{
    using Infrastructure;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Weee.Requests.AatfReturn;

    public class ValidateReturnEditActionFilterAttribute : ValidateReturnBaseActionFilterAttribute
    {
        public override async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid returnId)
        {
            using (var client = Client())
            {
                var @returnStatus = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetReturnStatus(returnId));

                if (@returnStatus.OtherInProgressReturn)
                {
                    filterContext.Result = AatfRedirect.ReturnsList(@returnStatus.OrganisationId);
                }
            }
        }
    }
}