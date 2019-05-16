namespace EA.Weee.Web.Areas.AatfReturn.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Services;

    public abstract class ValidateReturnBaseActionFilterAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public ConfigurationService ConfigService { get; set; }

        public abstract Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid returnId);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ConfigService.CurrentConfiguration.EnableAATFReturns)
            {
                throw new InvalidOperationException("AATF returns are not enabled.");
            }

            if (!context.RouteData.Values.TryGetValue("returnId", out var returnIdActionParameter))
            {
                throw new ArgumentException("No return ID was specified.");
            }

            if (!(Guid.TryParse(returnIdActionParameter.ToString(), out var guidReturnIdActionParameter)))
            {
                throw new ArgumentException("The specified return ID is not valid.");
            }

            var returnId = (Guid)guidReturnIdActionParameter;

            AsyncHelpers.RunSync(() => OnAuthorizationAsync(context, returnId));

            base.OnActionExecuting(context);
        }
    }
}