namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using System.Web.Routing;
    using EA.Weee.Web.Filters;

    public class CheckCanCreateTransferNoteAttribute : CheckTransferNoteAttributeBase
    {
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

            if (!context.ActionParameters.TryGetValue("complianceYear", out idActionParameter))
            {
                throw new ArgumentException("No compliance year was specified.");
            }

            if (!(int.TryParse(idActionParameter.ToString(), out var complianceYearIdActionParameter)))
            {
                throw new ArgumentException("The specified compliance year is not valid.");
            }

            AsyncHelpers.RunSync(() => OnAuthorizationAsync(context, pcsIdActionParameter, complianceYearIdActionParameter));
        }

        private static void RedirectToManageEvidence(ActionExecutingContext context)
        {
            context.Result = new RedirectToRouteResult(new RouteValueDictionary()
            {
                { "action", "Index" },
                { "controller", "ManageEvidenceNotes" }
            });
        }

        private async Task OnAuthorizationAsync(ActionExecutingContext filterContext, Guid pcsId, int complianceYear)
        {
            try
            {
                var scheme = await Cache.FetchSchemePublicInfo(pcsId);

                var currentDate = GetCurrentDate(filterContext.HttpContext);

                ValidateSchemeAndWindow(scheme, complianceYear, currentDate);
            }
            catch (InvalidOperationException)
            {
                RedirectToManageEvidence(filterContext);
            }
        }
    }
}