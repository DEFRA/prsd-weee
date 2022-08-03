namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class CheckCanCreateTransferNoteAttribute : CheckSchemeNoteAttributeBase
    {
        public override async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid pcsId)
        {
            var complianceYear = TryGetComplianceYear(filterContext);

            var scheme = await Cache.FetchSchemePublicInfo(pcsId);
            var currentDate = await GetCurrentDate(filterContext.HttpContext);

            ValidateSchemeAndWindow(scheme, complianceYear, currentDate);
        }
    }
}