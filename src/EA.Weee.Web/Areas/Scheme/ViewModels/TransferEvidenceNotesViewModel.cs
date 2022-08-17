namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Web.ViewModels.Shared;

    public class TransferEvidenceNotesViewModel : TransferEvidenceViewModelBase, IValidatableObject
    {
        public List<GenericControlPair<Guid, bool>> SelectedEvidenceNotePairs { get; set; }

        public ActionEnum Action { get; set; }

        public TransferEvidenceNotesViewModel()
        {
            SelectedEvidenceNotePairs = new List<GenericControlPair<Guid, bool>>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!SelectedEvidenceNotePairs.Any(s => s.Value.Equals(true)))
            {
                yield return new ValidationResult("Select at least one evidence note to transfer from", new[] { nameof(SelectedEvidenceNotePairs) });
            }
            else
            {
                if (SelectedEvidenceNotePairs.Count(s => s.Value.Equals(true)) > 5)
                {
                    yield return new ValidationResult("You cannot select more than 5 notes",
                        new[] { nameof(SelectedEvidenceNotePairs) });
                }
            }
        }
    }
}