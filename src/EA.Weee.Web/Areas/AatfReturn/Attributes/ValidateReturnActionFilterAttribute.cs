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
    
    public class ValidateReturnActionFilterAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public ConfigurationService ConfigService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            AsyncHelpers.RunSync(() => OnAuthorizationAsync(context));
        }

        public async Task OnAuthorizationAsync(ActionExecutingContext filterContext)
        {
            if (!ConfigService.CurrentConfiguration.EnableAATFReturns)
            {
                throw new InvalidOperationException("AATF returns are not enabled.");
            }

            if (!filterContext.RouteData.Values.TryGetValue("returnId", out var returnIdActionParameter))
            {
                throw new ArgumentException("No return ID was specified.");
            }

            if (!(Guid.TryParse(returnIdActionParameter.ToString(), out var guidReturnIdActionParameter)))
            {
                throw new ArgumentException("The specified return ID is not valid.");
            }

            var returnId = (Guid)guidReturnIdActionParameter;

            using (var client = Client())
            {
                var @returnStatus = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetReturnStatus(returnId));

                if (@returnStatus.ReturnStatus != ReturnStatus.Created)
                {
                    filterContext.Result = AatfRedirect.ReturnsList(@returnStatus.OrganisationId);
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}