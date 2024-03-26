namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class AatfAeDetailsViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Display(Name = "PCS, AATF or AE")]
        public string SelectedFacilityType { get; set; }

        public IEnumerable<SelectListItem> FacilityTypes { get; set; }

        [Display(Name = "Appropriate authority")]
        public Guid? CompetentAuthorityId { get; set; }

        public IEnumerable<SelectListItem> CompetentAuthoritiesList { get; set; }

        [Display(Name = "WROS Pan Area Team")]
        public Guid? PanAreaId { get; set; }
        public IEnumerable<SelectListItem> PanAreaList { get; set; }

        [Display(Name = "EA Area")]
        public Guid? LocalAreaId { get; set; }

        public IEnumerable<SelectListItem> LocalAreaList { get; set; }
    }
}