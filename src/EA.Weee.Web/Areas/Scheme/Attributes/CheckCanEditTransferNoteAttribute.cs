namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Api.Client;
    using EA.Weee.Web.Filters;
    using EA.Weee.Web.Services.Caching;
    using Infrastructure;
    using Weee.Requests.AatfEvidence;

    public class CheckCanEditTransferNoteAttribute : CheckTransferNoteAttributeBase
    {
        public IWeeeCache Cache { get; set; }

        public Func<IWeeeClient> Client { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionParameters.TryGetValue("pcsId", out var idActionParameter))
            {
                throw new ArgumentException("No pcs ID was specified.");
            }

            if (!(Guid.TryParse(idActionParameter.ToString(), out var pcsIdActionParameter)))
            {
                throw new ArgumentException("The specified organisation ID is not valid.");
            }

            if (!context.ActionParameters.TryGetValue("evidenceNoteId", out idActionParameter))
            {
                throw new ArgumentException("No compliance year was specified.");
            }

            if (!(Guid.TryParse(idActionParameter.ToString(), out var evidenceNoteIdActionParameter)))
            {
                throw new ArgumentException("The specified compliance year is not valid.");
            }

            AsyncHelpers.RunSync(() => OnAuthorizationAsync(context, pcsIdActionParameter, evidenceNoteIdActionParameter));
        }

        private static void RedirectToManageEvidence(ActionExecutingContext context)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary()
            {
                { "action", "Index" },
                { "controller", "ManageEvidenceNotes" }
            });
        }

        private async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid pcsId, Guid evidenceNoteId)
        {
            try
            {
                using (var client = Client())
                {
                    var note = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));
                    var scheme = await Cache.FetchSchemePublicInfo(pcsId);
                    var currentDate = await Cache.FetchCurrentDate();

                    ValidateSchemeAndWindow(scheme, note.ComplianceYear, currentDate);
                }
            }
            catch (InvalidOperationException)
            {
                RedirectToManageEvidence(filterContext);
            }
        }
    }
}