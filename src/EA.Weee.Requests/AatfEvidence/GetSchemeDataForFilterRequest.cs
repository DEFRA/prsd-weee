namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using EA.Weee.Core.Shared;
    using Prsd.Core.Mediator;

    [Serializable]
    public class GetSchemeDataForFilterRequest : IRequest<List<EntityIdDisplayNameData>>
    {
        public Guid? AatfId { get; private set; }

        public int ComplianceYear { get; private set; }

        public RecipientOrTransfer RecipientOrTransfer { get; private set; }

        public GetSchemeDataForFilterRequest(RecipientOrTransfer recipientOrTransfer,  Guid? aatfId, int complianceYear)
        {
            AatfId = aatfId;
            ComplianceYear = complianceYear;
            RecipientOrTransfer = recipientOrTransfer;
        }
    }
}
