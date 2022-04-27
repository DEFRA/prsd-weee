namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Core.AatfEvidence;
    using Core.DataReturns;
    using CuttingEdge.Conditions;
    using Prsd.Core.Mediator;

    public class GetEvidenceNotesForTransferRequest : IRequest<List<EvidenceNoteData>>
    {
        public Guid OrganisationId { get; protected set; }

        public List<WeeeCategory> Categories { get; private set; }

        public GetEvidenceNotesForTransferRequest(Guid organisationId, List<WeeeCategory> categories)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(categories).IsNotEmpty();

            OrganisationId = organisationId;
            Categories = categories;
        }
    }
}
