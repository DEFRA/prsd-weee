namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Core.Helpers;
    using Core.Scheme;
    using Core.Shared;
    using Filters;

    public abstract class CheckTransferNoteAttributeBase : CheckNoteAttributeBase
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var pcsId = TryGetPcsId(context);

            AsyncHelpers.RunSync(async () =>
            {
                try
                {
                    await OnAuthorizationAsync(context, pcsId);
                }
                catch
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary()
                    {
                        { "action", "Index" },
                        { "controller", "ManageEvidenceNotes" }
                    });
                }
            });

            base.OnActionExecuting(context);
        }

        public void ValidateSchemeAndWindow(SchemePublicInfo scheme, int complianceYear, DateTime date)
        {
            if (scheme.Status == SchemeStatus.Withdrawn)
            {
                throw new InvalidOperationException(
                    $"Evidence for organisation ID {scheme.OrganisationId} cannot be managed as scheme is withdrawn");
            }

            if (!WindowHelper.IsDateInComplianceYear(complianceYear, date))
            {
                throw new InvalidOperationException(
                    $"Evidence for organisation ID {scheme.OrganisationId} cannot be managed as not in current compliance year");
            }
        }
    }
}