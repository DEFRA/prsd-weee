﻿namespace EA.Weee.Web.Areas.Admin.ViewModels.SchemeReports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class ReportsFilterViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        [DisplayName("PCS name or direct registrants")]
        public Guid? SelectedScheme { get; set; }

        [DisplayName("Appropriate authority")]
        public Guid? SelectedAA { get; set; }

        public bool IncludeRemovedProducer { get; set; }

        public bool IncludeBrandNames { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        public IEnumerable<SelectListItem> SchemeNames { get; set; }

        public IEnumerable<SelectListItem> AppropriateAuthorities { get; set; }

        public ReportsFilterViewModel()
        {
        }
    }
}