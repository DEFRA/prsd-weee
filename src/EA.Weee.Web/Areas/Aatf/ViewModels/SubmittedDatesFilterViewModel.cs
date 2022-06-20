namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Attributes;

    [Serializable]
    public class SubmittedDatesFilterViewModel
    {
        [Display(Name = "Submitted date")]
        [DataType(DataType.Date)]
        [EvidenceNoteStartDate(nameof(EndDate), false)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Submitted date")]
        [DataType(DataType.Date)]
        [EvidenceNoteEndDate(nameof(StartDate), false)]
        public DateTime? EndDate { get; set; }
    }
}