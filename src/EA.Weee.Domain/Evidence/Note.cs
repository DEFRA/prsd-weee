namespace EA.Weee.Domain.Evidence
{
    using AatfReturn;
    using Organisation;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;
    using System;
    using System.Collections.Generic;
    using CuttingEdge.Conditions;
    using User;

    public partial class Note : Entity
    {
        private const string StatusTransitionError = @"Evidence note cannot transition from status '{0}' to '{1}'";

        public Note()
        {
        }

        public Note(Organisation organisation,
            Scheme recipient, 
            DateTime startDate,
            DateTime endDate,
            WasteType? wasteType,
            Protocol? protocol,
            Aatf aatf, 
            string createdBy,
            IList<NoteTonnage> tonnages)
        {
            Guard.ArgumentNotNull(() => organisation, organisation);
            Guard.ArgumentNotNull(() => recipient, recipient);
            Guard.ArgumentNotNull(() => aatf, aatf);
            Guard.ArgumentNotDefaultValue(() => startDate, startDate);
            Guard.ArgumentNotDefaultValue(() => endDate, endDate);
            Guard.ArgumentNotNullOrEmpty(() => createdBy, createdBy);
            Guard.ArgumentNotNull(() => tonnages, tonnages);

            Organisation = organisation;
            WasteType = wasteType;
            Protocol = protocol;
            Recipient = recipient;
            StartDate = startDate;
            EndDate = endDate;
            Aatf = aatf;
            CreatedById = createdBy;
            NoteType = NoteType.EvidenceNote;
            CreatedDate = SystemTime.UtcNow;
            Status = NoteStatus.Draft;
            ComplianceYear = (short)startDate.Year;
            NoteTonnage = tonnages;
            NoteStatusHistory = new List<NoteStatusHistory>();
            NoteTransferTonnage = new List<NoteTransferTonnage>();
        }

        public Note(Organisation organisation,
            Scheme recipient,
            string createdBy,
            IList<NoteTransferTonnage> transfer,
            IList<NoteTransferCategory> categories)
        {
            Guard.ArgumentNotNull(() => organisation, organisation);
            Guard.ArgumentNotNull(() => recipient, recipient);
            Guard.ArgumentNotNullOrEmpty(() => createdBy, createdBy);
            Guard.ArgumentNotNull(() => transfer, transfer);
            Guard.ArgumentNotNull(() => categories, categories);

            Organisation = organisation;
            Recipient = recipient;
            StartDate = SystemTime.UtcNow;
            EndDate = SystemTime.UtcNow;
            CreatedById = createdBy;
            NoteType = NoteType.TransferNote;
            CreatedDate = SystemTime.UtcNow;
            Status = NoteStatus.Draft;
            NoteTonnage = new List<NoteTonnage>();
            NoteStatusHistory = new List<NoteStatusHistory>();
            NoteTransferTonnage = transfer;
            NoteTransferCategories = categories;
        }

        public void Update(Scheme recipient, DateTime startDate, DateTime endDate, WasteType? wasteType,
            Protocol? protocol)
        {
            Guard.ArgumentNotNull(() => recipient, recipient);
            Guard.ArgumentNotDefaultValue(() => startDate, startDate);
            Guard.ArgumentNotDefaultValue(() => endDate, endDate);

            WasteType = wasteType;
            Protocol = protocol;
            Recipient = recipient;
            StartDate = startDate;
            EndDate = endDate;
        }

        public virtual void UpdateStatus(NoteStatus newStatus, string changedBy, string reason = null)
        {
            if (newStatus.Equals(NoteStatus.Draft) && Status.Equals(NoteStatus.Draft))
            {
                ThrowInvalidStateTransitionError(newStatus);
            }

            if ((newStatus.Equals(NoteStatus.Submitted) && Status.Equals(NoteStatus.Submitted)))
            {
                ThrowInvalidStateTransitionError(newStatus);
            }

            if ((newStatus.Equals(NoteStatus.Submitted) && 
                 (Status != NoteStatus.Draft && Status != NoteStatus.Returned)))
            {
                ThrowInvalidStateTransitionError(newStatus);
            }

            if ((newStatus.Equals(NoteStatus.Approved) && !Status.Equals(NoteStatus.Submitted)))
            {
                ThrowInvalidStateTransitionError(newStatus);
            }

            NoteStatusHistory.Add(new NoteStatusHistory(changedBy, Status, newStatus, reason));

            Status = newStatus;
        }

        private void ThrowInvalidStateTransitionError(NoteStatus newStatus)
        {
            throw new InvalidOperationException(string.Format(StatusTransitionError, Status, newStatus));
        }

        /// <summary>
        /// Should only be used for integration tests
        /// </summary>
        /// <param name="organisation"></param>
        public void UpdateOrganisation(Guid organisationId)
        {
            Guard.ArgumentNotDefaultValue(() => organisationId, organisationId);

            OrganisationId = organisationId;
            Organisation = null;
        }
        public void UpdateAatf(Guid aatfId)
        {
            Guard.ArgumentNotDefaultValue(() => aatfId, aatfId);

            AatfId = aatfId;
            Aatf = null;
        }

        public void UpdateScheme(Guid schemeId)
        {
            Guard.ArgumentNotDefaultValue(() => schemeId, schemeId);

            RecipientId = schemeId;
            Recipient = null;
        }

        public virtual Guid OrganisationId { get; set; }

        public virtual Guid? AatfId { get; set; }

        public virtual Guid RecipientId { get; set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual Scheme Recipient { get; private set; }

        public virtual DateTime StartDate { get; private set; }

        public virtual DateTime EndDate { get; private set; }

        public virtual WasteType? WasteType { get; private set; }

        public virtual NoteStatus Status { get; private set; }

        public virtual Protocol? Protocol { get; private set; }

        public virtual NoteType NoteType { get; private set; }

        public virtual Aatf Aatf { get; private set; }

        public virtual DateTime CreatedDate { get; private set; }

        public virtual string CreatedById { get; private set; }

        public virtual User CreatedBy { get; set; }

        public virtual int Reference { get; set; }

        public virtual ICollection<NoteTonnage> NoteTonnage { get; protected set; }

        public virtual ICollection<NoteStatusHistory> NoteStatusHistory { get; protected set; }

        public virtual ICollection<NoteTransferTonnage> NoteTransferTonnage { get; protected set; }

        public virtual short ComplianceYear { get; set; }

        public virtual ICollection<NoteTransferCategory> NoteTransferCategories { get; protected set; }
    }
}
