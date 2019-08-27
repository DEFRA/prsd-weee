namespace EA.Weee.Web.Areas.Admin.ViewModels.Charge
{
    using Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class IssuedChargesViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public int SelectedComplianceYear { get; set; }

        [DisplayName("PCS name")]
        public Guid? SelectedScheme { get; set; }

        public IEnumerable<int> ComplianceYears { get; set; }

        public IEnumerable<SchemeData> Schemes { get; set; }
    }
}