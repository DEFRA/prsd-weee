namespace EA.Weee.Domain.Evidence
{
    using System;
    using System.Collections.Generic;
    using AatfReturn;
    using Organisation;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;
    using User;

    public sealed partial class Note : Entity
    {
        private Note()
        {
            NoteTonnage = new List<NoteTonnage>();
        }

        public Note(Organisation organisation,
            Scheme recipient, 
            DateTime startDate,
            DateTime endDate,
            WasteType? wasteType,
            Protocol? protocol,
            Aatf aatf,
            NoteType noteType,
            string createdBy,
            NoteStatus status)
        {
            Guard.ArgumentNotNull(() => organisation, organisation);
            Guard.ArgumentNotNull(() => recipient, recipient);
            Guard.ArgumentNotNull(() => aatf, aatf);
            Guard.ArgumentNotNull(() => noteType, noteType);
            Guard.ArgumentNotNull(() => status, status);
            Guard.ArgumentNotDefaultValue(() => startDate, startDate);
            Guard.ArgumentNotDefaultValue(() => endDate, endDate);
            Guard.ArgumentNotNullOrEmpty(() => createdBy, createdBy);

            Organisation = organisation;
            Status = status;
            WasteType = wasteType;
            Protocol = protocol;
            Recipient = recipient;
            StartDate = startDate;
            EndDate = endDate;
            Aatf = aatf;
            CreatedById = createdBy;
            NoteType = noteType;
            CreatedDate = SystemTime.UtcNow;

            NoteTonnage = new List<NoteTonnage>();
        }

        /// <summary>
        /// Should only be used for integration tests
        /// </summary>
        /// <param name="organisation"></param>
        public void UpdateOrganisation(Organisation organisation)
        {
            Guard.ArgumentNotNull(() => organisation, organisation);

            Organisation = organisation;
        }

        public Organisation Organisation { get; private set; }

        public Scheme Recipient { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public WasteType? WasteType { get; private set; }

        public NoteStatus Status { get; private set; }

        public Protocol? Protocol { get; private set; }

        public NoteType NoteType { get; private set; }

        public Aatf Aatf { get; private set; }

        public DateTime CreatedDate { get; private set; }

        public DateTime? SubmittedDate { get; set; }

        public string CreatedById { get; private set; }

        public string SubmittedById { get; set; }

        public User CreatedBy { get; set; }

        public User SubmittedBy { get; set; }

        public int? Reference { get; set; }

        public ICollection<NoteTonnage> NoteTonnage { get; set; }
    }
}
