namespace EA.Weee.Web.Areas.Admin.ViewModels.AatfReports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.DataStandards;

    public class NonObligatedWeeeReceivedAtAatfsViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Display(Name = "Appropriate authority")]
        public Guid? CompetentAuthorityId { get; set; }

        public IEnumerable<SelectListItem> CompetentAuthoritiesList { get; set; }

        [Display(Name = "WROS Pan area team")]
        public Guid? PatAreaId { get; set; }
        public IEnumerable<SelectListItem> PatAreaList { get; set; }

        [DisplayName("AATF name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string AatfName { get; set; }
    }
}