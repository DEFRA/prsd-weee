namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using EA.Weee.Core.Organisations;
    using Shared;
    using System.ComponentModel.DataAnnotations;

    public class PreviousRegistrationViewModel : YesNoChoiceViewModel
    {
        [Required(ErrorMessage = "Have you previously been registered as a small producer?")]
        public override string SelectedValue { get; set; }

        public PreviousRegistrationViewModel()
            : base(CreateFromEnum<YesNoType>().PossibleValues)
        {
        }
    }
}