namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Admin;

    public class ProducerDetailsViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        [DisplayName("PCS name")]
        public Guid? SelectedScheme { get; set; }

        [DisplayName("Appropriate authority")]
        public Guid? SelectedAA { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        public IEnumerable<SelectListItem> SchemeNames { get; set; }

        public IEnumerable<SelectListItem> AppropriateAuthorities { get; set; }
    }
}