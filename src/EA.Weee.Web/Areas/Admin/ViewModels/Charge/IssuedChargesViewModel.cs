namespace EA.Weee.Web.Areas.Admin.ViewModels.Charge
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Web;

    public class IssuedChargesViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedComplianceYear { get; set; }

        [DisplayName("PCS name")]
        public string SelectedSchemeName { get; set; }

        public IEnumerable<int> ComplianceYears { get; set; }

        public IEnumerable<string> SchemeNames { get; set; }
    }
}