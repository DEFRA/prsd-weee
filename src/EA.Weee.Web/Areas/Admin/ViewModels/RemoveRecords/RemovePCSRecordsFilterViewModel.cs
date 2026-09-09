namespace EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class RemovePCSRecordsFilterViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public string SelectedYear { get; set; }

        [Required(ErrorMessage = "Select a PCS")]
        [DisplayName("PCS name")]
        public string SelectedScheme { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        public IEnumerable<SelectListItem> SchemeNames { get; set; }

        public RemovePCSRecordsListViewModel Results { get; set; }
    }
}