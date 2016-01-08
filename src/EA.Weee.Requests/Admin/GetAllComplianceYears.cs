namespace EA.Weee.Requests.Admin
{
    using System.Collections.Generic;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class GetAllComplianceYears : IRequest<List<int>>
    {
        public ComplianceYearFor ComplianceYearFor { get; set; }

        public GetAllComplianceYears(ComplianceYearFor complianceYearFor = ComplianceYearFor.MemberRegistrations)
        {
            ComplianceYearFor = complianceYearFor;
        }
    }
}
