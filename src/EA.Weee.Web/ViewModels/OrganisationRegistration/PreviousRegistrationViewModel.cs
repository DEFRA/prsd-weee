namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using Shared;
    using System.ComponentModel.DataAnnotations;

    public class PreviousRegistrationViewModel : YesNoChoiceViewModel
    {
        [Required(ErrorMessage = "Have you previously been registered as a small producer?")]
        public override string SelectedValue { get; set; }

        public string SearchText {get; set; }
    }
}