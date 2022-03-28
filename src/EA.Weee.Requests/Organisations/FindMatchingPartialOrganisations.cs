namespace EA.Weee.Requests.Organisations
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;

    public class FindMatchingPartialOrganisations : IRequest<OrganisationSearchDataResult>
    {
        public string CompanyName { get; private set; }

        public FindMatchingPartialOrganisations(string companyName)
        {
            Guard.ArgumentNotNullOrEmpty(() => companyName, companyName);

            CompanyName = companyName;
        }
    }
}
