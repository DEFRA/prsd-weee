namespace EA.Weee.Web.Areas.Admin.ViewModels.AatfReports
{
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class AatfSentOnDataViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Display(Name = "Appropriate authority")]
        public Guid? CompetentAuthorityId { get; set; }

        public IEnumerable<SelectListItem> CompetentAuthoritiesList { get; set; }

        [Display(Name = "WROS Pan Area Team")]
        public Guid? PanAreaId { get; set; }
        public IEnumerable<SelectListItem> PanAreaList { get; set; }

        [Display(Name = "Obligation type")]
        public ObligationType? SelectedObligationType { get; set; }

        public IEnumerable<SelectListItem> ObligationTypes
        {
            get
            {
                yield return new SelectListItem() { Text = "B2B" };
                yield return new SelectListItem() { Text = "B2C" };
            }
        }
    }
}