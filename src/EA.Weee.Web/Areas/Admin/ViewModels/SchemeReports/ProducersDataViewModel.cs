namespace EA.Weee.Web.Areas.Admin.ViewModels.SchemeReports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Shared;

    public class ProducersDataViewModel
    {
        private readonly string selectedSchemeDisplay;

        public ProducersDataViewModel(string selectedSchemeDisplay = "PCS name or direct registrants")
        {
            this.selectedSchemeDisplay = selectedSchemeDisplay;
        }

        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        //[DisplayName("PCS name or direct registrants")]
        public Guid? SelectedSchemeId { get; set; }

        public string GetSchemeDisplayName()
        {
            return selectedSchemeDisplay;
        }

        public IEnumerable<SelectListItem> Schemes { get; set; }

        [Required(ErrorMessage = "Select an obligation type")]
        [Display(Name = "Obligation type")]
        public ObligationType SelectedObligationType { get; set; }

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