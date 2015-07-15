namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Type
{
    using System;
    using Core.Organisations;
    using Shared;

    public class OrganisationTypeViewModel
    {
        public Guid? OrganisationId { get; set; }

        public RadioButtonStringCollectionViewModel OrganisationTypes { get; set; }

        public OrganisationTypeViewModel()
        {
            OrganisationTypes = RadioButtonStringCollectionViewModel.CreateFromEnum<OrganisationType>();
        }

        public OrganisationTypeViewModel(OrganisationType organisationType, Guid orgId)
        {
            OrganisationTypes = RadioButtonStringCollectionViewModel.CreateFromEnum<OrganisationType>(organisationType);
            OrganisationId = orgId;
        }
    }
}