namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using Constant;
    using Filters;
    using Helpers;
    using Infrastructure;
    using Services;
    using Weee.Requests.AatfReturn;

    public class CheckCanCreateEvidenceNoteAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public IAatfEvidenceHelper AatfEvidenceHelper { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionParameters.TryGetValue("organisationId", out var idActionParameter))
            {
                throw new ArgumentException("No organisation ID was specified.");
            }

            if (!(Guid.TryParse(idActionParameter.ToString(), out var organisationIdActionParameter)))
            {
                throw new ArgumentException("The specified organisation ID is not valid.");
            }

            if (!context.ActionParameters.TryGetValue("aatfId", out idActionParameter))
            {
                throw new ArgumentException("No aatf ID was specified.");
            }

            if (!(Guid.TryParse(idActionParameter.ToString(), out var aatfIdActionParameter)))
            {
                throw new ArgumentException("The specified aatf ID is not valid.");
            }

            if (!context.ActionParameters.TryGetValue("complianceYear", out idActionParameter))
            {
                throw new ArgumentException("No compliance year was specified.");
            }

            if (!(int.TryParse(idActionParameter.ToString(), out var complianceYearActionParameter)))
            {
                throw new ArgumentException("The specified compliance year is not valid.");
            }

            AsyncHelpers.RunSync(() => OnAuthorizationAsync(context, organisationIdActionParameter, aatfIdActionParameter, complianceYearActionParameter));
        }

        private async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid organisationId, Guid aatfId, int complianceYear)
        {
            using (var client = Client())
            {
                var allAatfsAndAes = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetAatfByOrganisation(organisationId));

                var canEdit = AatfEvidenceHelper.AatfCanEditCreateNotes(allAatfsAndAes, aatfId, complianceYear);

                if (!canEdit)
                {
                    throw new InvalidOperationException($"Evidence for organisation ID {organisationId} with aatf ID {aatfId} cannot be created");
                }
            }
        }
    }
}