namespace EA.Weee.Requests.Admin
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.Shared;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetAllAatfsForComplianceYearRequest : IRequest<List<EntityIdDisplayNameData>>
    {
        public int ComplianceYear { get; private set; }

        public GetAllAatfsForComplianceYearRequest(int complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}
