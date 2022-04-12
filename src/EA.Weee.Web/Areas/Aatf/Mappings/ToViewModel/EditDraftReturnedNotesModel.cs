﻿namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using System;

    public class EditDraftReturnedNotesModel 
    {
        public EditDraftReturnedNotesModel(int referenceId, 
            string recipient, 
            NoteStatus status, 
            WasteType? wasteType,
            Guid id,
            NoteType type)
        {
            Guard.ArgumentNotDefaultValue(() => referenceId, referenceId);

            Recipient = recipient;
            ReferenceId = referenceId;
            Status = status;
            WasteType = wasteType;
            Id = id;
            Type = type;
        }

        public EditDraftReturnedNotesModel(int referenceId, string recipient, NoteStatus status, WasteType? wasteType, Guid id,
            NoteType noteType, DateTime? submittedDate, string submittedBy) : this(referenceId, recipient, status, wasteType, id, noteType)
        {
            SubmittedDate = submittedDate;
            SubmittedBy = submittedBy;
        }

        public int ReferenceId { get; set; }

        public string Recipient { get; set; }

        public Guid Id { get; protected set; }

        public NoteType Type { get; protected set; }
        public NoteStatus Status { get; set; }
        
        public WasteType? WasteType { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public string SubmittedBy { get; set; }
    }
}
