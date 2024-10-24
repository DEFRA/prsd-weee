﻿namespace EA.Weee.Domain.Evidence
{
    using AatfReturn;
    using Organisation;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CuttingEdge.Conditions;
    using User;

    public partial class Note : Entity
    {
        private const string StatusTransitionError = @"Evidence note cannot transition from status '{0}' to '{1}'";

        public Note()
        {
        }

        public Note(Organisation organisation,
            Organisation recipient, 
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
            ComplianceYear = startDate.Year;
            NoteTonnage = tonnages;
            NoteStatusHistory = new List<NoteStatusHistory>();
            NoteTransferTonnage = new List<NoteTransferTonnage>();
        }

        public Note(Organisation organisation,
            Organisation recipient,
            string createdBy,
            IList<NoteTransferTonnage> transfer,
            int complianceYear)
        {
            Guard.ArgumentNotNull(() => organisation, organisation);
            Guard.ArgumentNotNull(() => recipient, recipient);
            Guard.ArgumentNotNullOrEmpty(() => createdBy, createdBy);
            Guard.ArgumentNotNull(() => transfer, transfer);
            Condition.Requires(complianceYear).IsGreaterThan(0);

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
            ComplianceYear = complianceYear;
            WasteType = Evidence.WasteType.HouseHold;
        }

        public void Update(Organisation recipient, DateTime startDate, DateTime endDate, WasteType? wasteType,
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

        public void Update(Organisation recipient)
        {
            Guard.ArgumentNotNull(() => recipient, recipient);

            Recipient = recipient;
        }

        public virtual void UpdateStatus(NoteStatus newStatus, string changedBy, DateTime date, string reason = null)
        {
            if (newStatus == NoteStatus.Draft && Status == NoteStatus.Draft)
            {
                ThrowInvalidStateTransitionError(newStatus);
            }

            if (newStatus == NoteStatus.Submitted && Status == NoteStatus.Submitted)
            {
                ThrowInvalidStateTransitionError(newStatus);
            }

            if (newStatus == NoteStatus.Submitted && (Status != NoteStatus.Draft && Status != NoteStatus.Returned))
            {
                ThrowInvalidStateTransitionError(newStatus);
            }

            if ((newStatus == NoteStatus.Approved || newStatus == NoteStatus.Rejected || newStatus == NoteStatus.Returned) 
                && Status != NoteStatus.Submitted)
            {
                ThrowInvalidStateTransitionError(newStatus);
            }

            if (newStatus == NoteStatus.Void && Status != NoteStatus.Approved)
            {
                ThrowInvalidStateTransitionError(newStatus);
            }

            NoteStatusHistory.Add(new NoteStatusHistory(changedBy, Status, newStatus, date, reason));

            Status = newStatus;
        }

        private void ThrowInvalidStateTransitionError(NoteStatus newStatus)
        {
            throw new InvalidOperationException(string.Format(StatusTransitionError, Status, newStatus));
        }

        public void SetApprovedRecipientAddress(string name, string address)
        {
            Condition.Requires(address).IsNotNullOrWhiteSpace();
            Condition.Requires(name).IsNotNullOrWhiteSpace();

            ApprovedRecipientAddress = address;
            ApprovedRecipientSchemeName = name;
        }

        public void SetApprovedTransfererAddress(string name, string address)
        {
            Condition.Requires(address).IsNotNullOrWhiteSpace();
            Condition.Requires(name).IsNotNullOrWhiteSpace();

            ApprovedTransfererAddress = address;
            ApprovedTransfererSchemeName = name;
        }

        public virtual Guid OrganisationId { get; set; }

        public virtual Guid? AatfId { get; set; }

        public virtual Guid RecipientId { get; set; }

        public virtual Organisation Organisation { get; private set; }

        public virtual Organisation Recipient { get; private set; }

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

        public IList<NoteTonnage> FilteredNoteTonnage(IList<int> categories)
        {
            return NoteTonnage.Where(nt => nt.Received.HasValue && categories.Contains((int)nt.CategoryId)).ToList();
        }

        public virtual ICollection<NoteStatusHistory> NoteStatusHistory { get; protected set; }

        public virtual ICollection<NoteTransferTonnage> NoteTransferTonnage { get; protected set; }

        public virtual int ComplianceYear { get; set; }

        public virtual string ApprovedRecipientAddress { get; protected set; }

        public virtual string ApprovedRecipientSchemeName { get; protected set; }

        public virtual string ApprovedTransfererAddress { get; protected set; }

        public virtual string ApprovedTransfererSchemeName { get; protected set; }
    }
}
