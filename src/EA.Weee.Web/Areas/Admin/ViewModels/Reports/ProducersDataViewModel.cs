namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Shared;

    public class ProducersDataViewModel
    {
        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [DisplayName("PCS name")]
        public Guid? SelectedSchemeId { get; set; }

        public IEnumerable<SelectListItem> Schemes { get; set; }

        [Required]
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