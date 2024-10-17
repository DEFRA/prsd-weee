namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using EA.Weee.Core.Organisations;
    using Shared;
    using System.ComponentModel.DataAnnotations;

    public class PreviousRegistrationViewModel : RadioButtonStringCollectionViewModel
    {
        [Required(ErrorMessage = "Have you previously been registered as a producer?")]
        public override string SelectedValue { get; set; }

        public PreviousRegistrationViewModel()
            : base(CreateFromEnum<PreviouslyRegisteredProducerType>().PossibleValues)
        {
        }
    }
}