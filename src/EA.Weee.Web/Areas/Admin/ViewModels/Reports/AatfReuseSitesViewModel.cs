namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    public class AatfReuseSitesViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Display(Name = "Appropriate authority")]
        public Guid? CompetentAuthorityId { get; set; }

        public IEnumerable<SelectListItem> CompetentAuthoritiesList { get; set; }

        [Display(Name = "WROS Pan area team")]
        public Guid? PanAreaId { get; set; }
        public IEnumerable<SelectListItem> PanAreaList { get; set; }

        [DisplayName("AATF name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string AATFName { get; set; }
    }
}