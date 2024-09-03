namespace EA.Weee.Web.Areas.Admin.ViewModels.AatfReports
{
    using Core.DataStandards;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class NonObligatedWeeeReceivedAtAatfViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [DisplayName("Organisation name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string OrganisationName { get; set; }
    }
}