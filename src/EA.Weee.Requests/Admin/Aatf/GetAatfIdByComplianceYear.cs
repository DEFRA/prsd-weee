namespace EA.Weee.Requests.Admin.Aatf
{
    using System;
    using Prsd.Core.Mediator;

    public class GetAatfIdByComplianceYear : IRequest<Guid>
    {
        public Guid AatfId { get; private set; }

        public int ComplianceYear { get; private set; }

        public GetAatfIdByComplianceYear(Guid aatfId, int complianceYear)
        {
            AatfId = aatfId;
            ComplianceYear = complianceYear;
        }
    }
}
