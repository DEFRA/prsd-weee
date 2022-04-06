namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using EA.Weee.Web.Areas.Aatf.ViewModels;
    using System;

    public class EditDraftReturnedNotesModel 
    {
        public EditDraftReturnedNotesModel(int referenceId, string recipient, NoteStatus status, WasteType? wasteType)
        {
            Guard.ArgumentNotDefaultValue(() => referenceId, referenceId);

            this.Recipient = recipient;
            this.ReferenceId = referenceId;
            this.Status = status;
            this.WasteType = wasteType;
        }

        public int ReferenceId { get; set; }

        public string Recipient { get; set; }

        public NoteStatus Status { get; set; }
        
        public WasteType? WasteType { get; set; }
    }
}
