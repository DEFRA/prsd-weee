namespace EA.Weee.Requests.Organisations.DirectRegistrant
{
    using System;
    using CuttingEdge.Conditions;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;

    public class AddRepresentingCompany : IRequest<Guid>
    {
        public Guid OrganisationId { get; private set; }

        public RepresentingCompanyDetailsViewModel RepresentingCompanyDetailsViewModel { get; private set; }

        public AddRepresentingCompany(Guid organisationId, RepresentingCompanyDetailsViewModel representingCompanyDetailsViewModel)
        {
            Condition.Requires(organisationId).IsNotEqualTo(Guid.Empty);
            Condition.Requires(representingCompanyDetailsViewModel).IsNotNull();

            OrganisationId = organisationId;
            RepresentingCompanyDetailsViewModel = representingCompanyDetailsViewModel;
        }
    }
}
