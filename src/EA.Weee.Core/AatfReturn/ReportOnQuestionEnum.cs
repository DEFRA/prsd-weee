namespace EA.Weee.Core.AatfReturn
{
    using System.ComponentModel.DataAnnotations;

    public enum ReportOnQuestionEnum
    {
        [Display(Name = "Weee Received")]
        WeeeReceived = 1,
        [Display(Name = "Weee Sent On")]
        WeeeSentOn = 2,
        [Display(Name = "Weee Reused")]
        WeeeReused = 3,
        [Display(Name = "Non Obligated")]
        NonObligated = 4,
        [Display(Name = "Non Obligated Dcf")]
        NonObligatedDcf = 5,
    }
}