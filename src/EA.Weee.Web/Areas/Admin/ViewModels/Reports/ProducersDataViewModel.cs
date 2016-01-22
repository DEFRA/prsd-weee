namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Shared;
    using Prsd.Core.Helpers;
    public class ProducersDataViewModel
    {
        public ProducersDataViewModel()
        {
            var obligationTypeNone = ObligationType.None.ToString();
            var obligationTypeBoth = ObligationType.Both.ToString();
            var allObligationTypes = new SelectList(EnumHelper.GetValues(typeof(ObligationType)), "Key", "Value");
            ObligationTypes = new SelectList(allObligationTypes.Where(x => (x.Text != obligationTypeNone && x.Text != obligationTypeBoth)).ToList(), "Value", "Text");
        }

        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Required]
        [Display(Name = "Obligation type")]
        public ObligationType SelectedObligationType { get; set; }

        public IEnumerable<SelectListItem> ObligationTypes { get; set; }
    }
}