namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Type
{
    using Core.Organisations;
    using Shared;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class OrganisationTypeViewModel : RadioButtonStringCollectionViewModel
    {
        [Required(ErrorMessage = "Select organisation type")]
        public override string SelectedValue { get; set; }

        public OrganisationTypeViewModel()
            : base(CreateFromEnum<ExternalOrganisationType>().PossibleValues)
        {
        }
    }
}