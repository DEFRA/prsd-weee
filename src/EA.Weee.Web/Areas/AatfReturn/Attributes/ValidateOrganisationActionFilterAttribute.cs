namespace EA.Weee.Web.Areas.AatfReturn.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Shared;
    using Services;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;

    public class ValidateOrganisationActionFilterAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public ConfigurationService ConfigService { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ConfigService.CurrentConfiguration.EnableAATFReturns)
            {
                throw new InvalidOperationException("AATF returns are not enabled.");
            }

            if (!context.RouteData.Values.TryGetValue("organisationId", out var organisationIdActionParameter))
            {
                throw new ArgumentException("No organisation ID was specified.");
            }

            if (!(Guid.TryParse(organisationIdActionParameter.ToString(), out var guidOrganisationIdActionParameter)))
            {
                throw new ArgumentException("The specified organisation ID is not valid.");
            }

            var organisationId = (Guid)guidOrganisationIdActionParameter;

            base.OnActionExecuting(context);
        }
    }
}