namespace EA.Weee.Requests.Shared
{
    using System.ComponentModel.DataAnnotations;

    public enum Status
    {
        [Display(Name = "InComplete")]
        InComplete = 1,
        [Display(Name = "Complete")]
        Complete = 2
    }
}
