namespace EA.Weee.Requests.AatfEvidence
{
    using System;
    using System.Collections.Generic;
    using Aatf;
    using Core.AatfEvidence;
    using Prsd.Core;
    using Prsd.Core.Mediator;

    public abstract class EvidenceNoteBaseRequest : IRequest<Guid>
    {
        protected EvidenceNoteBaseRequest()
        {
        }

        protected EvidenceNoteBaseRequest(Guid organisationId,
            Guid aatfId,
            Guid recipientId,
            DateTime startDate,
            DateTime endDate,
            WasteType? wasteType,
            Protocol? protocol,
            List<TonnageValues> tonnages,
            NoteStatus status,
            Guid id)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);
            Guard.ArgumentNotDefaultValue(() => recipientId, recipientId);
            Guard.ArgumentNotDefaultValue(() => startDate, startDate);
            Guard.ArgumentNotDefaultValue(() => endDate, endDate);

            OrganisationId = organisationId;
            RecipientId = recipientId;
            AatfId = aatfId;
            StartDate = startDate;
            EndDate = endDate;
            WasteType = wasteType;
            Protocol = protocol;
            TonnageValues = tonnages;
            Status = status;
            Id = id;
        }

        public IList<TonnageValues> TonnageValues { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public WasteType? WasteType { get; set; }

        public Protocol? Protocol { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid AatfId { get; set; }

        public Guid RecipientId { get; set; }

        public NoteStatus Status { get; set; }

        public Guid Id { get;  set; }
    }
}