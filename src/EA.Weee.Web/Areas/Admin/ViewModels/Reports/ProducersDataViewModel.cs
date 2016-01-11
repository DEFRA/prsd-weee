namespace EA.Weee.Web.Areas.Admin.ViewModels.Reports
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web.Mvc;
    using Core.DataReturns;
    using Core.Shared;
    using Prsd.Core.Helpers;
    public class ProducersDataViewModel
    {
        public ProducersDataViewModel()
        {
            var obligationTypeNone = ObligationType.None.ToString();
            var obligationTypeBoth = ObligationType.Both.ToString();
            var allObligationTypes = new SelectList(EnumHelper.GetValues(typeof(ObligationType)), "Key", "Value");
            Obligationtypes = new SelectList(allObligationTypes.Where(x => (x.Text != obligationTypeNone && x.Text != obligationTypeBoth)).ToList(), "Value", "Text");
        }

        [Required]
        [DisplayName("Compliance year")]
        public int SelectedYear { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        [Required]
        [Display(Name = "Obligation type")]
        public ObligationType SelectedObligationtype { get; set; }

        public IEnumerable<SelectListItem> Obligationtypes { get; set; }
    }
}