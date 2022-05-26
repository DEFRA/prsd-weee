namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Core.Scheme;

    public class RecipientWasteStatusFilterBase
    {
        public List<SchemeData> SchemeList { get; set; }

        public Guid? ReceivedId { get; set; }

        public WasteType? WasteType { get; set; }

        public NoteStatus? NoteStatus { get; set; }

        public RecipientWasteStatusFilterBase(List<SchemeData> schemeList, Guid? receivedId, WasteType? wasteType, NoteStatus? noteStatus)
        {
            SchemeList = schemeList;
            ReceivedId = receivedId;
            WasteType = wasteType;
            NoteStatus = noteStatus;
        }
    }
}
