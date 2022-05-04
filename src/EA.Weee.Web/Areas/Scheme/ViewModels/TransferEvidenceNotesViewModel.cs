namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Core.Shared;
    using Web.ViewModels.Shared;

    public class TransferEvidenceNotesViewModel : TransferEvidenceViewModelBase, IValidatableObject
    {
        public List<CategoryValue> CategoryValues { get; set; }

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
            else
            {
                if (SelectedEvidenceNotes.Count(s => s.Equals(true)) > 5)
                {
                    yield return new ValidationResult("You cannot select more than 5 notes",
                        new[] { nameof(SelectedEvidenceNotes) });
                }
            }
        }
    }
}