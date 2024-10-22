namespace EA.Weee.Web.ViewModels.Organisation.Mapping.ToViewModel
{
    using CuttingEdge.Conditions;
    using EA.Weee.Api.Client.Models;
    using EA.Weee.Core.Organisations;
    using System.Collections.Generic;

    public class RepresentingCompaniesViewModelMapSource
    {
        public IList<OrganisationUserData> OrganisationsData { get; private set; }

        public Core.Organisations.OrganisationData OrganisationData { get; private set; }

        public RepresentingCompaniesViewModelMapSource(IList<OrganisationUserData> organisationsData, Core.Organisations.OrganisationData organisationData)
        {
            Condition.Requires(organisationsData).IsNotEmpty();
            Condition.Requires(organisationData).IsNotNull();

            OrganisationsData = organisationsData;
            OrganisationData = organisationData;
        }
    }
}