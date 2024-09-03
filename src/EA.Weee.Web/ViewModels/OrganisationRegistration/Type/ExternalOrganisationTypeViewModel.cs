namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Type
{
    using Core.Organisations;
    using EA.Weee.Core.Shared;
    using Shared;
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ExternalOrganisationTypeViewModel : RadioButtonStringCollectionViewModel
    {
        [Required(ErrorMessage = "Select organisation type")]
        public override string SelectedValue { get; set; }

        public ExternalOrganisationTypeViewModel()
            : base(CreateFromEnum<ExternalOrganisationType>().PossibleValues)
        {
        }
    }
}