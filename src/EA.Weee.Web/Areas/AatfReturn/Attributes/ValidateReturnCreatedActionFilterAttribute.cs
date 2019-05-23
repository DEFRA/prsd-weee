namespace EA.Weee.Web.Areas.AatfReturn.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.AatfReturn;
    using Core.Shared;
    using Infrastructure;
    using Services;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;
    
    public class ValidateReturnCreatedActionFilterAttribute : ValidateReturnBaseActionFilterAttribute
    {
        public override async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid returnId)
        {
            using (var client = Client())
            {
                var @returnStatus = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetReturnStatus(returnId));

                if (@returnStatus.ReturnStatus != ReturnStatus.Created)
                {
                    filterContext.Result = AatfRedirect.ReturnsList(@returnStatus.OrganisationId);
                }
            }
        }
    }
}