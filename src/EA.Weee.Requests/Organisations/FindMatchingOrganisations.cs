namespace EA.Weee.Requests.Organisations
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Mediator;

    public class FindMatchingOrganisations : IRequest<OrganisationSearchDataResult>
    {
        public string CompanyName { get; private set; }

        public bool Paged { get; private set; }

        public int Page { get; private set; }

        public int OrganisationsPerPage { get; set; }

        public FindMatchingOrganisations(string companyName, int? page = null, int? organisationsPerPage = null)
        {
            Guard.ArgumentNotNullOrEmpty(() => companyName, companyName);
    
            CompanyName = companyName;

            if (page.HasValue && organisationsPerPage.HasValue)
            {
                Paged = true;
                Page = page.Value;
                OrganisationsPerPage = organisationsPerPage.Value;
            }
        }
    }
}