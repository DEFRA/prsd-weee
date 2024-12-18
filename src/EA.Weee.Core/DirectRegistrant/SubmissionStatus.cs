namespace EA.Weee.Core.DirectRegistrant
{
    using System.ComponentModel.DataAnnotations;
    public enum SubmissionStatus
    {
        [Display(Name = "Incomplete")]
        InComplete = 1,

        [Display(Name = "Submitted")]
        Submitted,

        [Display(Name = "Returned")]
        Returned
    }
}
