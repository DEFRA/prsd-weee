namespace EA.Weee.Requests.Organisations
{
    using Core.Organisations;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;

    public class FindMatchingOrganisations : IRequest<OrganisationSearchDataResult>
    {
        public string CompanyName { get; private set; }

        public FindMatchingOrganisations(string companyName)
        {
            Guard.ArgumentNotNullOrEmpty(() => companyName, companyName);
    
            CompanyName = companyName;
        }
    }
}