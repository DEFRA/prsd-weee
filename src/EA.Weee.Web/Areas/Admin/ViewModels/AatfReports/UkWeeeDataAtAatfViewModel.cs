﻿namespace EA.Weee.Web.Areas.Admin.ViewModels.AatfReports
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Shared;

    public class UkWeeeDataAtAatfViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Required]
        [Display(Name = "Obligation type")]
        public ObligationType SelectedObligationType { get; set; }
    }
}