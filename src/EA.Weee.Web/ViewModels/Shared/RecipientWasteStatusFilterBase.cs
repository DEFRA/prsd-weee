﻿namespace EA.Weee.Web.Areas.Aatf.ViewModels
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

        public Guid? SubmittedBy { get; set; }

        public List<EntityIdDisplayNameData> SubmittedByList { get; set; }

        public bool AllStatuses { get; set; }

        public RecipientWasteStatusFilterBase(List<EntityIdDisplayNameData> recipientList, Guid? receivedId, WasteType? wasteType, NoteStatus? noteStatus, 
            Guid? submittedBy, List<EntityIdDisplayNameData> submittedByList, bool internalUser, bool allStatuses)
        {
            RecipientList = recipientList;
            ReceivedId = receivedId;
            WasteType = wasteType;
            NoteStatus = noteStatus;
            SubmittedBy = submittedBy;
            SubmittedByList = submittedByList;
            Internal = internalUser;
            AllStatuses = allStatuses;
        }
    }
}