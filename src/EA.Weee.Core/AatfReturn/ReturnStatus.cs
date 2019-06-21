namespace EA.Weee.Core.AatfReturn
{
    using System.ComponentModel.DataAnnotations;

    public enum ReturnStatus
    {
        [Display(Name = "Not Submitted")]
        Created = 1,
        [Display(Name = "Submitted")]
        Submitted = 2
    }
}
