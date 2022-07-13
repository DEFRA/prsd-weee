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

    public class CheckCanEditEvidenceAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public IAatfEvidenceHelper AatfEvidenceHelper { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            object idActionParameter;

            if (context.ActionParameters.TryGetValue("viewModel", out var viewModel))
            {
                var convertedModel = viewModel as EvidenceNoteViewModel;

                if (convertedModel == null)
                {
                    throw new ArgumentException("Edit evidence note view model incorrect type.");
                }

                idActionParameter = convertedModel.Id;
            }
            else
            {
                if (!context.RouteData.Values.TryGetValue("evidenceNoteId", out idActionParameter))
                {
                    throw new ArgumentException("No evidence note ID was specified.");
                }
            }

            if (!(Guid.TryParse(idActionParameter.ToString(), out var evidenceNoteIdActionParameter)))
            {
                throw new ArgumentException("The specified evidence note ID is not valid.");
            }

            var evidenceNoteId = (Guid)evidenceNoteIdActionParameter;

            AsyncHelpers.RunSync(() => OnAuthorizationAsync(context, evidenceNoteId));

            base.OnActionExecuting(context);
        }

        private async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid evidenceNoteId)
        {
            using (var client = Client())
            {
                var evidenceNoteData = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetEvidenceNoteForAatfRequest(evidenceNoteId));

                if ((!evidenceNoteData.Status.Equals(NoteStatus.Draft)
                     && !evidenceNoteData.Status.Equals(NoteStatus.Returned)))
                {
                    throw new InvalidOperationException($"Evidence note {evidenceNoteData.Id} is incorrect state to be edited");
                }

                var allAatfsAndAes = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetAatfByOrganisation(evidenceNoteData.AatfData.Organisation.Id));

                var canEdit = AatfEvidenceHelper.AatfCanEditCreateNotes(allAatfsAndAes, evidenceNoteData.AatfData.Id,
                    evidenceNoteData.ComplianceYear);

                if (!canEdit)
                {
                    throw new InvalidOperationException($"Evidence note {evidenceNoteData.Id} cannot edit notes");
                }
            }
        }
    }
}