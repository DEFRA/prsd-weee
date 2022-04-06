namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using EA.Weee.Core.AatfEvidence;
   
    public class EditDraftReturnedNote 
    {
        public int ReferenceId { get; set; }

        public string Recipient { get; set; }

        public WasteType? TypeOfWaste { get; set; }

        public NoteStatus Status { get; set; }

        public Guid Id { get; set; }
    }
}
