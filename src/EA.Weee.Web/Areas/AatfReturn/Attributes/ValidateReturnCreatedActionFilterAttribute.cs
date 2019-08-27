namespace EA.Weee.Web.Areas.AatfReturn.Attributes
{
    using Core.AatfReturn;
    using Infrastructure;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Weee.Requests.AatfReturn;

    public class ValidateReturnCreatedActionFilterAttribute : ValidateReturnBaseActionFilterAttribute
    {
        public override async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid returnId)
        {
            using (var client = Client())
            {
                var @return = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetReturn(returnId, false));

                if (@return.ReturnStatus != ReturnStatus.Created)
                {
                    filterContext.Result = AatfRedirect.ReturnsList(@return.OrganisationId);
                }

                if (!@return.QuarterWindow.IsOpen(@return.SystemDateTime))
                {
                    filterContext.Result = new RedirectResult("~/errors/QuarterClosed");
                }
            }
        }
    }
}