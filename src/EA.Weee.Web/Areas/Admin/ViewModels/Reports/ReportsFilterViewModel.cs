namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class ReportsFilterViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        [DisplayName("PCS name")]
        public Guid? SelectedScheme { get; set; }

        [DisplayName("Appropriate authority")]
        public Guid? SelectedAA { get; set; }
        
        public bool IncludeRemovedProducer { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        public IEnumerable<SelectListItem> SchemeNames { get; set; }

        public IEnumerable<SelectListItem> AppropriateAuthorities { get; set; }

        public bool FilterByScheme { get; private set; }

        public ReportsFilterViewModel()
            : this(true)
        {
        }

        public ReportsFilterViewModel(bool filterByScheme)
        {
            FilterByScheme = filterByScheme;
        }
    }
}