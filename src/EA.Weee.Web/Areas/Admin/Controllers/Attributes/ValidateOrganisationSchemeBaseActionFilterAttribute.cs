namespace EA.Weee.Web.Areas.Admin.Controllers.Attributes
{
    using EA.Weee.Web.Filters;
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
                throw new ArgumentException("No organisation ID was specified.");
            }

            Guid organisationId = (Guid)organisationIdParameter;

            AsyncHelpers.RunSync(() => OnAuthorizationAsync(context, organisationId));

            base.OnActionExecuting(context);
        }
    }
}