namespace EA.Weee.Web.Areas.Scheme.ViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Web.ViewModels.Shared;

    public class OrganisationUsersViewModel
    {
        [Required]
        [Display(Name = "Organisation users")]
        public StringGuidRadioButtons OrganisationUsers { get; set; }
    }
}