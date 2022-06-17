namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Attributes;
    using Core.AatfEvidence;
    using Core.Helpers;
    using Core.Scheme;
    using Web.ViewModels.Shared;

    [Serializable]
    public class SubmittedDatesFilterViewModel
    {
        [Display(Name = "Submitted date")]
        [DataType(DataType.Date)]
        [EvidenceNoteStartDate(nameof(EndDate), false)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Submitted date")]
        [DataType(DataType.Date)]
        [EvidenceNoteEndDate(nameof(StartDate))]
        public DateTime? EndDate { get; set; }
    }
}