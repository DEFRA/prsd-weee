namespace EA.Weee.Domain.Evidence
{
    using System;
    using AatfReturn;
    using Organisation;
    using Prsd.Core;
    using Prsd.Core.Domain;
    using Scheme;
    using User;

    public partial class Note : Entity
    {
        protected Note()
        {
        }

        public Note(Organisation organisation,
            Scheme recipient, 
            DateTime startDate,
            DateTime endDate,
            WasteType wasteType,
            Protocol protocol,
            Aatf aatf,
            NoteType noteType,
            string createdBy)
        {
            Guard.ArgumentNotNull(() => organisation, organisation);
            Guard.ArgumentNotNull(() => recipient, recipient);
            Guard.ArgumentNotNull(() => aatf, aatf);
            Guard.ArgumentNotNull(() => noteType, noteType);
            Guard.ArgumentNotDefaultValue(() => startDate, startDate);
            Guard.ArgumentNotDefaultValue(() => endDate, endDate);
            Guard.ArgumentNotNullOrEmpty(() => createdBy, createdBy);

            Organisation = organisation;
            Status = NoteStatus.Draft;
            WasteType = wasteType;
            Protocol = protocol;
            Recipient = recipient;
            StartDate = startDate;
            EndDate = endDate;
            Aatf = aatf;
            CreatedById = createdBy;
            NoteType = noteType;
            CreatedDate = SystemTime.UtcNow;
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

        public virtual Organisation Organisation { get; private set; }

        public virtual Scheme Recipient { get; private set; }

        public virtual DateTime StartDate { get; private set; }

        public virtual DateTime EndDate { get; private set; }

        public virtual WasteType WasteType { get; private set; }

        public virtual NoteStatus Status { get; private set; }

        public virtual Protocol Protocol { get; private set; }

        public virtual NoteType NoteType { get; private set; }

        public virtual Aatf Aatf { get; private set; }

        public virtual DateTime CreatedDate { get; private set; }

        public virtual DateTime? SubmittedDate { get; set; }

        public virtual string CreatedById { get; private set; }

        public virtual string SubmittedById { get; set; }

        public virtual User CreatedBy { get; set; }

        public virtual User SubmittedBy { get; set; }

        public virtual int Reference { get; set; }
    }
}
