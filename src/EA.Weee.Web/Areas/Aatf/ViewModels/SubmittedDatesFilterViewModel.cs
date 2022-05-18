namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Web.Areas.Aatf.Attributes;

    public class SubmittedDatesFilterViewModel
    {
        [DisplayName("Submitted date")]
        [DataType(DataType.Date)]
        [EvidenceNoteStartDate(nameof(EndDate))]
        public DateTime? StartDate { get; set; }

        [DisplayName("Submitted date")]
        [DataType(DataType.Date)]
        [EvidenceNoteEndDate(nameof(StartDate))]
        public DateTime? EndDate { get; set; }
    }
}