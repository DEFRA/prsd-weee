namespace EA.Weee.Web.Areas.Admin.ViewModels.RemoveRecords
{
    using EA.Weee.Core.Shared;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PCSFilteringViewModel
    {
        [DisplayName("PCS name")]
        public string PCSName { get; set; }

        [DisplayName("Compliance year")]
        public int? ComplianceYear { get; set; }
    }
}