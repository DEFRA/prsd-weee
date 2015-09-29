namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Type
{
    using System;
    using Core.Organisations;
    using Shared;

    public class OrganisationTypeViewModel
    {
        public Guid? OrganisationId { get; set; }

        public string SearchedText { get; set; }

        public RadioButtonStringCollectionViewModel OrganisationTypes { get; set; }

        public OrganisationTypeViewModel()
        {
            OrganisationTypes = RadioButtonStringCollectionViewModel.CreateFromEnum<OrganisationType>();
            SearchedText = string.Empty;
        }

        public OrganisationTypeViewModel(string searchText)
        {
            OrganisationTypes = RadioButtonStringCollectionViewModel.CreateFromEnum<OrganisationType>();
            SearchedText = searchText;
        }

        public OrganisationTypeViewModel(OrganisationType organisationType, Guid orgId)
        {
            OrganisationTypes = RadioButtonStringCollectionViewModel.CreateFromEnum<OrganisationType>(organisationType);
            SearchedText = string.Empty;
            OrganisationId = orgId;
        }
    }
}