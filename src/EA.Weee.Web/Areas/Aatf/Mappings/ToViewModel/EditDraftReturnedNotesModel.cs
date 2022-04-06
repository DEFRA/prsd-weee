namespace EA.Weee.Web.Areas.Aatf.Mappings.ToViewModel
{
    using EA.Prsd.Core;
    using EA.Weee.Core.AatfEvidence;
    using ViewModels;
    using System;
    using EA.Weee.Core.Helpers;

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

        public int ReferenceId { get; protected set; }

        public string Recipient { get; protected set; }

        public NoteStatus Status { get; protected set; }
        
        public WasteType? WasteType { get; protected set; }

        public Guid Id { get; protected set; }

        public NoteType Type { get; protected set; }
    }
}
