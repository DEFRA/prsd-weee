namespace EA.Weee.Web.ViewModels.Shared
{
    using EA.Weee.Core.AatfEvidence;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class RecipientWasteStatusFilterViewModel
    {
        [Display(Name = "Recipient")]
        public Guid? ReceivedId { get; set; }

        public IEnumerable<SelectListItem> RecipientList { get; set; }

        [Display(Name = "Obligation type")]
        public WasteType? WasteTypeValue { get; set; }

        public IEnumerable<SelectListItem> WasteTypeList { get; set; }

        [Display(Name = "Status")]
        public NoteStatus? NoteStatusValue { get; set; }

        public IEnumerable<SelectListItem> NoteStatusList { get; set; }

        [Display(Name = "Submitted by")]
        public Guid? SubmittedBy { get; set; }

        public IEnumerable<SelectListItem> SubmittedByList { get; set; }

        [Display(Name = "Evidence type")]
        public EvidenceNoteType? EvidenceNoteTypeValue { get; set; }

        public IEnumerable<SelectListItem> EvidenceTypeList { get; set; }

        public IEnumerable<SelectListItem> EvidenceNoteTypeList { get; set; }

        public bool SearchPerformed => NoteStatusValue.HasValue || WasteTypeValue.HasValue || ReceivedId.HasValue || EvidenceNoteTypeValue.HasValue;
    }
}