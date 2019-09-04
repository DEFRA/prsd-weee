namespace EA.Weee.Requests.Scheme
{
    using Core.Scheme;
    using Core.Shared;
    using Prsd.Core.Mediator;
    using System;

    public class CreateScheme : IRequest<CreateOrUpdateSchemeInformationResult>
    {
        public CreateScheme(Guid organisationId, string schemeName, string approvalNumber, string ibisCustomerReference, ObligationType obligationType, Guid competentAuthorityId, SchemeStatus status)
        {
            OrganisationId = organisationId;
            SchemeName = schemeName;
            ApprovalNumber = approvalNumber;
            IbisCustomerReference = ibisCustomerReference;
            ObligationType = obligationType;
            CompetentAuthorityId = competentAuthorityId;
            Status = status;
        }

        public Guid OrganisationId { get; set; }

        public string SchemeName { get; set; }

        public string ApprovalNumber { get; set; }

        public string IbisCustomerReference { get; set; }

        public ObligationType ObligationType { get; set; }

        public Guid CompetentAuthorityId { get; set; }

        public SchemeStatus Status { get; set; }
    }
}
