namespace EA.Weee.Requests.Scheme
{
    using System;
    using Core.Scheme.MemberUploadTesting;
    using Prsd.Core.Mediator;

    public class UpdateSchemeInformation : IRequest<Guid>
    {
        public UpdateSchemeInformation(Guid schemeId, string schemeName, string ibisCustomerReference, ObligationType obligationType, Guid competentAuthorityId)
        {
            SchemeId = schemeId;
            SchemeName = schemeName;
            IbisCustomerReference = ibisCustomerReference;
            ObligationType = obligationType;
            CompetentAuthorityId = competentAuthorityId;
        }

        public Guid SchemeId { get; set; }

        public string SchemeName { get; set; }

        public string IbisCustomerReference { get; set; }

        public ObligationType ObligationType { get; set; }

        public Guid CompetentAuthorityId { get; set; }
    }
}
