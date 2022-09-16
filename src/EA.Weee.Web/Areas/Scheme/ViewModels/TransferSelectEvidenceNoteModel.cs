namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Constant;

    public class TransferSelectEvidenceNoteModel : TransferSelectEvidenceNoteModelBase, IValidatableObject
    {
        public int NumberOfSelectedNotes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // if number of selected notes is already 5 and we are trying to add
            if (NumberOfSelectedNotes >= 5)
            {
                yield return new ValidationResult("You cannot select more than 5 notes", new[] { ValidationKeyConstants.TransferEvidenceNotesSelectedNotesError });
            }
        }

        public int NewPage
        {
            get
            {
                if (PageCount != 1)
                {
                    return Page;
                }

                if (Page > 1)
                {
                    return Page - 1;
                }

                return 1;
            }
        }
    }
}