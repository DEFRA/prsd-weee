namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using EA.Weee.Core.Organisations;
    using Shared;
    using System.ComponentModel.DataAnnotations;

    public class AuthorisedRepresentativeViewModel : YesNoChoiceViewModel
    {
        [Required(ErrorMessage = "Please indicate whether or not you are applying as an authorised representative of a non-UK established company")]
        public override string SelectedValue { get; set; }

        public string SearchText {get; set; }

        public AuthorisedRepresentativeViewModel()
            : base(CreateFromEnum<YesNoType>().PossibleValues)
        {
        }
    }
}