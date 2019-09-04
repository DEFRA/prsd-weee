namespace EA.Weee.Web.Areas.Admin.Controllers.Attributes
{
    using EA.Weee.Api.Client;
    using EA.Weee.Core.Scheme;
    using EA.Weee.Requests.Scheme;
    using EA.Weee.Web.Areas.AatfReturn.Attributes;
    using EA.Weee.Web.Infrastructure;
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public abstract class ValidateOrganisationSchemeBaseActionFilterAttribute : ActionFilterAttribute
    {
        public abstract Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid organisationId);

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionParameters.TryGetValue("organisationId", out var organisationIdParameter))
            {
                throw new ArgumentException("No return ID was specified.");
            }

            Guid organisationId = (Guid)organisationIdParameter;

            AsyncHelpers.RunSync(() => OnAuthorizationAsync(context, organisationId));

            base.OnActionExecuting(context);
        }
    }
}