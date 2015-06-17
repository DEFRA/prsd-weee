namespace EA.Weee.Requests.Organisations
{
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;

    public class FindMatchingOrganisations : IRequest<IList<OrganisationSearchData>>
    {
        public string CompanyName { get; private set; }

        public bool Paged { get; private set; }

        public int Page { get; private set; }

        public int OrganisationsPerPage { get; set; }

        public FindMatchingOrganisations(string companyName)
        {
            Guard.ArgumentNotNullOrEmpty(() => companyName, companyName);
    
            CompanyName = companyName;
            Paged = false;
        }

        public FindMatchingOrganisations(string companyName, int page, int organisationsPerPage)
        {
            Guard.ArgumentNotNullOrEmpty(() => companyName, companyName);

            CompanyName = companyName;
            Paged = true;
            Page = page;
            OrganisationsPerPage = organisationsPerPage;
        }
    }
}