namespace EA.Weee.Web.ViewModels.Shared
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.Areas.Aatf.Attributes;

    [Serializable]
    public class SubmittedDatesFilterViewModel
    {
        [Display(Name = "Submitted start date")]
        [DataType(DataType.Date)]
        [EvidenceNoteFilterStartDate(nameof(EndDate))]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Submitted end date")]
        [DataType(DataType.Date)]
        [EvidenceNoteFilterEndDate(nameof(StartDate))]
        public DateTime? EndDate { get; set; }

        public bool SearchPerformed => EndDate.HasValue || StartDate.HasValue;
    }
}