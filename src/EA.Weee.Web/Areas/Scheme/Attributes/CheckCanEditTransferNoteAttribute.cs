namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Infrastructure;
    using Weee.Requests.AatfEvidence;

    public class CheckCanEditTransferNoteAttribute : CheckSchemeNoteAttributeBase
    {
        public override async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid pcsId)
        {
            var evidenceNoteId = TryGetEvidenceNoteId(filterContext);

            using (var client = Client())
            {
                var note = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), 
                    new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var scheme = await Cache.FetchSchemePublicInfo(pcsId);
                var currentDate = await GetCurrentDate(filterContext.HttpContext);

                ValidateSchemeAndWindow(scheme, note.ComplianceYear, currentDate);
            }
        }
    }
}