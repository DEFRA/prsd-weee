﻿namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;
    using Constant;
    using Web.ViewModels.Shared;
    using Weee.Requests.Scheme;

    [Serializable]
    public class TransferEvidenceNotesViewModel : TransferEvidenceViewModelBase, IValidatableObject
    {
        public List<GenericControlPair<Guid, bool>> SelectedEvidenceNotePairs { get; set; }

        public ActionEnum Action { get; set; }

        public int? PageNumber { get; set; }

        public int NoteCount { get; set; }

        public TransferEvidenceNotesViewModel()
        {
            SelectedEvidenceNotePairs = new List<GenericControlPair<Guid, bool>>();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            //if (!PageNumber.HasValue)
            //{
            //    var transferRequest =
            //        (TransferEvidenceNoteRequest)HttpContext.Current.Session[SessionKeyConstant.TransferNoteKey];

            //    if (!SelectedEvidenceNotePairs.Any(s => s.Value.Equals(true)))
            //    {
            //        if (!transferRequest.EvidenceNoteIds.Any())
            //        {
            //            yield return new ValidationResult("Select at least one evidence note to transfer from", new[] { nameof(SelectedEvidenceNotePairs) });
            //        }
            //    }
            //    else
            //    {
            //        if (SelectedEvidenceNotePairs.Count(s => s.Value.Equals(true)) > 5 || transferRequest.EvidenceNoteIds.Count > 5)
            //        {
            //            yield return new ValidationResult("You cannot select more than 5 notes",
            //                new[] { nameof(SelectedEvidenceNotePairs) });
            //        }
            //    }
            //}

            yield return ValidationResult.Success;
        }
    }
}