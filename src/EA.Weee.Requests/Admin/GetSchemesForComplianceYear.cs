namespace EA.Weee.Requests
{
    using System.Collections.Generic;
    using Prsd.Core.Mediator;

    public class GetSchemesForComplianceYear : IRequest<List<string>>
    {
        public int? ComplianceYear { get; set; }

        public GetSchemesForComplianceYear(int? complianceYear)
        {
            ComplianceYear = complianceYear;
        }
    }
}
