namespace EA.Weee.Core.AatfEvidence
{
    using System.ComponentModel.DataAnnotations;

    public enum TonnageToDisplayReportEnum
    {
        [Display(Name = "Original Tonnages")]
        OriginalTonnages = 1,
        [Display(Name = "Net of transfers")]
        Net = 2
    }
}
