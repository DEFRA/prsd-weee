namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Shared;

    public class RecipientWasteStatusFilterBase
    {
        public bool Internal { get; set; }

        public List<EntityIdDisplayNameData> RecipientList { get; set; }

        public Guid? ReceivedId { get; set; }

        public WasteType? WasteType { get; set; }

        public NoteStatus? NoteStatus { get; set; }

        public EvidenceNoteType? EvidenceNoteType { get; set; }

        public Guid? SubmittedBy { get; set; }

        public List<EntityIdDisplayNameData> SubmittedByList { get; set; }

        public bool AllStatuses { get; set; }

        public List<NoteStatus> NoteStatuseList { get; set; }

        public RecipientWasteStatusFilterBase(List<EntityIdDisplayNameData> recipientList, Guid? receivedId, WasteType? wasteType, NoteStatus? noteStatus, 
            Guid? submittedBy, List<EntityIdDisplayNameData> submittedByList, EvidenceNoteType? evidenceNoteType, bool internalUser, bool allStatuses, List<NoteStatus> noteStatuseList = null)
        {
            RecipientList = recipientList;
            ReceivedId = receivedId;
            WasteType = wasteType;
            NoteStatus = noteStatus;
            SubmittedBy = submittedBy;
            SubmittedByList = submittedByList;
            EvidenceNoteType = evidenceNoteType;
            Internal = internalUser;
            AllStatuses = allStatuses;
            NoteStatuseList = noteStatuseList;
        }
    }
}
