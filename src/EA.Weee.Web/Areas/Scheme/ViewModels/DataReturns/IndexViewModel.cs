namespace EA.Weee.Web.Areas.Scheme.ViewModels.DataReturns
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core;

    public class IndexViewModel
    {
        public Guid OrganisationId { get; private set; }

        public IReadOnlyCollection<int> ComplianceYears { get; private set; }

        public IndexViewModel(Guid organisationId, IReadOnlyCollection<int> complianceYears)
        {
            Guard.ArgumentNotNull(() => complianceYears, complianceYears);

            OrganisationId = organisationId;
            ComplianceYears = complianceYears;
        }
    }
} 