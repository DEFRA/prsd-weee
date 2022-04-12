namespace EA.Weee.Web.Areas.Aatf.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Api.Client;
    using Core.AatfEvidence;
    using Filters;
    using Infrastructure;
    using ViewModels;
    using Weee.Requests.AatfEvidence;

    public class CheckEditEvidenceNoteStatusAttribute : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            object idActionParameter = 0;

            if (context.ActionParameters.TryGetValue("viewModel", out var viewModel))
            {
                var convertedModel = viewModel as EvidenceNoteViewModel;

                if (convertedModel == null)
                {
                    throw new ArgumentException("Edit evidence note view model incorrect type");
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
                var evidenceNoteData = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetEvidenceNoteRequest(evidenceNoteId));

                if (!evidenceNoteData.Status.Equals(NoteStatus.Draft))
                {
                    throw new InvalidOperationException($"Evidence note {evidenceNoteData.Id} is incorrect state to be edited");
                }
            }
        }
    }
}