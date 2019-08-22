namespace EA.Weee.Requests.Admin.Aatf
{
    using Core.Admin;
    using Prsd.Core.Mediator;
    using System;

    public class GetAatfObligatedData : IRequest<CSVFileData>
    {
        public int ComplianceYear { get; private set; }

        public int Quarter { get; private set; }

        public Guid ReturnId { get; private set; }

        public Guid AatfId { get; private set; }

        public GetAatfObligatedData(int complianceYear,
         int quarter, Guid returnId, Guid aatfId)
        {
            ComplianceYear = complianceYear;
            Quarter = quarter;
            ReturnId = returnId;
            AatfId = aatfId;
        }
    }
}
