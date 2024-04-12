namespace EA.Weee.Web.Areas.Admin.ViewModels.AatfReports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class AatfAePublicRegisterViewModel
    {
        [Display(Name = "Appropriate authority")]
        public Guid? CompetentAuthorityId { get; set; }

        public IEnumerable<SelectListItem> CompetentAuthoritiesList { get; set; }

        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Required(ErrorMessage = "Select AATF or AE")]
        [Display(Name = "AATF or AE")]
        public string SelectedFacilityType { get; set; }

        public IEnumerable<SelectListItem> FacilityTypes { get; set; }
    }
}