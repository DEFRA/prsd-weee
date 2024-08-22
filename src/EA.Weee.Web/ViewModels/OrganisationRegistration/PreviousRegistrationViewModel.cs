namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using Core.Organisations;
    using Shared;
    using System.ComponentModel.DataAnnotations;

    public class PreviousRegistrationViewModel : YesNoChoiceViewModel
    {
        [Required(ErrorMessage = "Have you previously had an account with NPWD?")]
        public override string SelectedValue { get; set; }
    }
}