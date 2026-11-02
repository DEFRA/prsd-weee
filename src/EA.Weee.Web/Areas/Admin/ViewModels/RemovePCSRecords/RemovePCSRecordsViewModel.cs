namespace EA.Weee.Web.Areas.Admin.ViewModels.RemovePCSRecords
{
    using EA.Weee.Core.Shared.Paging;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public class RemovePCSRecordsViewModel
    {
        [Required(ErrorMessage = "Select a compliance year")]
        [DisplayName("Compliance year")]
        public string SelectedYear { get; set; }

        [Required(ErrorMessage = "Select a PCS")]
        [DisplayName("PCS name")]
        public string SelectedScheme { get; set; }

        public IEnumerable<SelectListItem> ComplianceYears { get; set; }

        public IEnumerable<SelectListItem> SchemeNames { get; set; }

        public IPagedList<RemovePCSRowViewModel> PCSSchemeData { get; set; }

        public RemovePCSRecordsViewModel()
        {
        }

        public RemovePCSRecordsViewModel(string selectedYear,
                                         string selectedScheme,
                                         IEnumerable<SelectListItem> complianceYears,
                                         IEnumerable<SelectListItem> schemeNames,
                                         IPagedList<RemovePCSRowViewModel> pcsSchemeData)
        {
            SelectedYear = selectedYear;
            SelectedScheme = selectedScheme;
            SchemeNames = schemeNames;
            ComplianceYears = complianceYears;
            PCSSchemeData = pcsSchemeData;
        }
    }
}