namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel.DataAnnotations;

    public enum YesNoType
    {
        [Display(Name = "Yes")]
        Yes = 1,

        [Display(Name = "No")]
        No = 2,
    }
}