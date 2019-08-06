namespace EA.Weee.Core.AatfReturn
{
    using System.ComponentModel.DataAnnotations;

    public enum ReportReturnStatus
    {
        [Display(Name = "Not Started")]
        NotStarted = 0,

        [Display(Name = "Started")]
        Started = 1,

        [Display(Name = "Submitted")]
        Submitted = 2
    }
}
