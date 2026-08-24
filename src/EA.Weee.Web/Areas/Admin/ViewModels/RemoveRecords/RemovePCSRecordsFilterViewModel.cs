namespace EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Core.Admin;
    using Core.Shared.Paging;
    using EA.Weee.Requests.Admin;

    public class RemovePCSRecordsFilterViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public string SelectedYear { get; set; }

        [Required(ErrorMessage = "Select a PCS")]
        [DisplayName("PCS name")]
        public Guid SelectedScheme { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        public IEnumerable<SelectListItem> SchemeNames { get; set; }

        public string OrderBy { get; set; }

        public RemovePCSRecordsFilterViewModel()
        {
        }
    }
}