namespace EA.Weee.Web.Areas.Admin.ViewModels.AatfReports
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Shared;

    public class PcsAatfDataDifferenceViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [DisplayName("Quarter")]
        public int? Quarter { get; set; }

        public IEnumerable<SelectListItem> Quarters
        {
            get
            {
                yield return new SelectListItem() { Text = "Q1", Value = "1" };
                yield return new SelectListItem() { Text = "Q2", Value = "2" };
                yield return new SelectListItem() { Text = "Q3", Value = "3" };
                yield return new SelectListItem() { Text = "Q4", Value = "4" };
            }
        }

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