﻿namespace EA.Weee.Requests.Scheme
{
    using System;
    using Core.Shared;
    using Prsd.Core.Mediator;

    public class UpdateSchemeInformation : IRequest<Guid>
    {
        public UpdateSchemeInformation(Guid schemeId, string schemeName, string approvalNumber, string ibisCustomerReference, ObligationType obligationType, Guid competentAuthorityId)
        {
            SchemeId = schemeId;
            SchemeName = schemeName;
            ApprovalNumber = approvalNumber;
            IbisCustomerReference = ibisCustomerReference;
            ObligationType = obligationType;
            CompetentAuthorityId = competentAuthorityId;
        }

        public Guid SchemeId { get; set; }

        public string SchemeName { get; set; }

        public string ApprovalNumber { get; set; }

        public string IbisCustomerReference { get; set; }

        public ObligationType ObligationType { get; set; }

        public Guid CompetentAuthorityId { get; set; }
    }
}
