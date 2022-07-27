﻿namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Infrastructure;
    using Weee.Requests.AatfEvidence;

    public class CheckCanEditTransferNoteAttribute : CheckTransferNoteAttributeBase
    {
        public override async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid pcsId)
        {
            var evidenceNoteId = TryGetEvidenceNoteId(filterContext);

            using (var client = Client())
            {
                var note = await client.SendAsync(filterContext.HttpContext.User.GetAccessToken(), 
                    new GetTransferEvidenceNoteForSchemeRequest(evidenceNoteId));

                var scheme = await Cache.FetchSchemePublicInfo(pcsId);
                var currentDate = await Cache.FetchCurrentDate();

                ValidateSchemeAndWindow(scheme, note.ComplianceYear, currentDate);
            }
        }
    }
}