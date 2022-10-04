﻿namespace EA.Weee.Web.Areas.Aatf.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using EA.Weee.Core.AatfEvidence;
   
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

        [Display(Name = "Submitted by")]
        public Guid? SubmittedBy { get; set; }

        public IEnumerable<SelectListItem> SubmittedByList { get; set; }

        public IEnumerable<SelectListItem> NoteStatusList { get; set; }
    }
}
