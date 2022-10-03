namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;

    public class RecipientWasteStatusFilterBase
    {
        public bool Internal { get; set; }

        public List<OrganisationSchemeData> RecipientList { get; set; }

        public Guid? ReceivedId { get; set; }

        public WasteType? WasteType { get; set; }

        public NoteStatus? NoteStatus { get; set; }

        public Guid? SubmittedBy { get; set; }

        public List<OrganisationSchemeData> SubmittedByList { get; set; }

        public RecipientWasteStatusFilterBase(List<OrganisationSchemeData> recipientList, Guid? receivedId, WasteType? wasteType, NoteStatus? noteStatus, 
            Guid? submittedBy, List<OrganisationSchemeData> submittedByList, bool internalUser)
        {
            RecipientList = recipientList;
            ReceivedId = receivedId;
            WasteType = wasteType;
            NoteStatus = noteStatus;
            SubmittedBy = submittedBy;
            SubmittedByList = submittedByList;
            Internal = internalUser;
        }
    }
}
