namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using Api.Client;
    using Core.Helpers;
    using Core.Scheme;
    using Core.Shared;
    using Filters;
    using Infrastructure;
    using Services.Caching;
    using Weee.Requests.Shared;

    public abstract class CheckTransferNoteAttributeBase : ActionFilterAttribute
    {
        public Func<IWeeeClient> Client { get; set; }

        public IWeeeCache Cache { get; set; }

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

        public DateTime GetCurrentDate(HttpContextBase httpContext)
        {
            return AsyncHelpers.RunSync(async () =>
            {
                using (var c = Client())
                {
                    return await c.SendAsync(httpContext.User.GetAccessToken(), new GetApiUtcDate());
                }
            });
        }
    }
}