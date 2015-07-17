namespace EA.Weee.Requests.PCS.MemberRegistration
{
    using System;
    using Prsd.Core.Mediator;

    public class GetProducerCSVByComplianceYear : IRequest<string>
    {
        public Guid OrganisationId { get; private set; }

        public int ComplianceYear { get; private set; }

        public GetProducerCSVByComplianceYear(Guid organisationId, int complianceYear)
        {
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}
