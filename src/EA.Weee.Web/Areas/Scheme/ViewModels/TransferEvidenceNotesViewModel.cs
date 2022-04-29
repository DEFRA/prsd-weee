namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Linq;
    using Core.Shared;
    using Web.ViewModels.Shared;

    public class TransferEvidenceNotesViewModel : IValidatableObject
    {
        public Guid PcsId { get; set; }

        public string RecipientName { get; set; }

        public List<CategoryValue> CategoryValues { get; set; }

        public List<ViewEvidenceNoteViewModel> EvidenceNotesDataList { get; set; }

        public List<bool> SelectedEvidenceNotes { get; set; }

        public TransferEvidenceNotesViewModel()
        {
            CategoryValues = new List<CategoryValue>();
            EvidenceNotesDataList = new List<ViewEvidenceNoteViewModel>();
            SelectedEvidenceNotes = new List<bool>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!SelectedEvidenceNotes.Any(s => s.Equals(true)))
            {
                yield return new ValidationResult("Select at least one evidence note to transfer from", new[] { nameof(SelectedEvidenceNotes) });
            }
        }
    }
}