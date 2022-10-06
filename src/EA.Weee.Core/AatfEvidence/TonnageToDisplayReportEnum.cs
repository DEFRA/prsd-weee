namespace EA.Weee.Core.AatfEvidence
{
    using System.ComponentModel.DataAnnotations;

    public enum TonnageToDisplayReportEnum
    {
        [Display(Name = "Original tonnages")]
        OriginalTonnages = 1,
        [Display(Name = "Net of transfers")]
        Net = 2
    }
}
