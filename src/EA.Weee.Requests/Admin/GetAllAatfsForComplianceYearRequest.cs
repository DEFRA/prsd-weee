namespace EA.Weee.Requests.Admin
{
    using System;
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetAllAatfsForComplianceYearRequest : IRequest<List<OrganisationSchemeData>>
    {
        public int ComplianceYear { get; private set; }

        public GetAllAatfsForComplianceYearRequest(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}
