namespace EA.Weee.Requests.Shared
{
    using System.ComponentModel.DataAnnotations;

    public enum NotificationType
    {
        [Display(Name = "Recovery")]
        Recovery = 1,
        [Display(Name = "Disposal")]
        Disposal = 2
    }
}