namespace EA.Weee.Web.Areas.Scheme.Attributes
{
    using System;
    using Core.Helpers;
    using Core.Scheme;
    using Core.Shared;
    using Filters;
    using Infrastructure;
    using Services.Caching;
    using Weee.Requests.Shared;

    public abstract class CheckSchemeNoteAttributeBase : CheckNoteAttributeBase
    {
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