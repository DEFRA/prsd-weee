namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Constant;
    using Web.ViewModels.Shared;

    [Serializable]
    public class TransferEvidenceNotesViewModel : TransferEvidenceViewModelBase, IValidatableObject
    {
        public ActionEnum Action { get; set; }

        public int PageNumber { get; set; }

        public int NoteCount { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!EvidenceNotesDataList.Any())
            {
                yield return new ValidationResult("Select at least one evidence note to transfer from", new[] { ValidationKeyConstants.TransferEvidenceNotesSelectedNotesError });
            }
            else
            {
                if (EvidenceNotesDataList.Count() > 5)
                {
                    yield return new ValidationResult("You cannot select more than 5 notes",
                        new[] { ValidationKeyConstants.TransferEvidenceNotesSelectedNotesError });
                }
            }
        }
    }
}