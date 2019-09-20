namespace EA.Weee.Core.Admin.AatfReports
{
    using System.ComponentModel.DataAnnotations;

    public enum ReportFacilityType
    {
        [Display(Name = "AATF")]
        Aatf = 1,
        [Display(Name = "AE")]
        Ae = 2,
        [Display(Name = "PCS")]
        Pcs = 3
    }
}
