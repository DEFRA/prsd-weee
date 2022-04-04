namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using System;

    public class EditDraftReturnedNotesModel
    {
        public EditDraftReturnedNotesModel(int referenceId, Guid recipientId, string status, string wasteType)
        {
            Guard.ArgumentNotDefaultValue(() => referenceId, referenceId);
            Guard.ArgumentNotDefaultValue(() => recipientId, recipientId);

            this.RecipientId = recipientId;
            this.ReferenceId = referenceId;
            this.Status = status;
            this.WasteType = wasteType;
        }

        public int ReferenceId { get; set; }

        public Guid RecipientId { get; set; }

        public string Status { get; set; }
        
        public string WasteType { get; set; }
    }
}
