namespace EA.Weee.Requests.Scheme.MemberRegistration
{
    using System;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetProducerCSV : IRequest<ProducerCSVFileData>
    {
        public Guid OrganisationId { get; private set; }
        public int ComplianceYear { get; private set; }

        public GetProducerCSV(Guid organisationId, int complianceYear)
        {
            OrganisationId = organisationId;
            ComplianceYear = complianceYear;
        }
    }
}
