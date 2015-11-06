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
         [Required(ErrorMessage = "Compliance year is required.")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        [DisplayName("Scheme name")]
        public Guid? SelectedScheme { get; set; }

        [DisplayName("AA")]
        public Guid? SelectedAA { get; set; }
       
        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        public IEnumerable<SelectListItem> SchemeNames { get; set; }

        public IEnumerable<SelectListItem> AppropriateAuthorities { get; set; }
    }
}