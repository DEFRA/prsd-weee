namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Attributes;

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
    }
}