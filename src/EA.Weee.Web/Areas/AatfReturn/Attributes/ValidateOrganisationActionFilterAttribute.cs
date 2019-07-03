namespace EA.Weee.Web.Areas.AatfReturn.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Shared;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Requests.AatfReturn;
    using EA.Weee.Web.Infrastructure;
    using Services;
    using Weee.Requests.Organisations;
    using Weee.Requests.Scheme;

    public class ValidateOrganisationActionFilterAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public ConfigurationService ConfigService { get; set; }

        public FacilityType FacilityType { get; set; }

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

            if (FacilityType != 0)
            {
                using (var client = Client())
                {
                    var aatfData = client.SendAsync(context.HttpContext.User.GetAccessToken(), new GetAatfByOrganisationFacilityType(organisationId, FacilityType));

                    if (aatfData.Result == null || aatfData.Result.Count == 0)
                    {
                        throw new InvalidOperationException(string.Format("No {0} found for this organisation.", this.FacilityType.ToString().ToUpper()));
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}