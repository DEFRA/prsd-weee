namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.AatfEvidence;
    using Filters;
    using Helpers;
    using Infrastructure;
    using Web.ViewModels.Shared;
    using Weee.Requests.AatfEvidence;
    using Weee.Requests.AatfReturn;
    using Weee.Requests.Shared;

    public class CheckCreateEvidenceNoteStatusAttribute : ActionFilterAttribute
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

            AsyncHelpers.RunSync(() => OnAuthorizationAsync(context, organisationIdActionParameter, aatfIdActionParameter));

            base.OnActionExecuting(context);
        }

        private async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid organisationId, Guid aatfId)
        {
            using (var client = Client())
            {
                var allAatfsAndAes = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetAatfByOrganisation(organisationId));

                var canEdit = AatfEvidenceHelper.AatfCanEditCreateNotes(allAatfsAndAes, aatfId, null);

                if (!canEdit)
                {
                    throw new InvalidOperationException($"Evidence for organisation ID {organisationId} with aatf ID {aatfId} cannot be created");
                }
            }
        }
    }
}