namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Aatf;
    using Core.AatfEvidence;
    using Prsd.Core.Mediator;

    public abstract class EvidenceNoteBaseRequest : IRequest<Guid>
    {
        public IList<TonnageValues> TonnageValues { get; set; }

        public DateTime StartDate { get; protected set; }

        public DateTime EndDate { get; protected set; }

        public WasteType? WasteType { get; protected set; }

        public Protocol? Protocol { get; protected set; }

        public Guid OrganisationId { get; protected set; }

        public Guid AatfId { get; protected set; }

        public Guid RecipientId { get; protected set; }
    }
}