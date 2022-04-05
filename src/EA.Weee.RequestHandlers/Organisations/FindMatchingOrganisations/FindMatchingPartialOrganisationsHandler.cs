namespace EA.Weee.RequestHandlers.Organisations.FindMatchingPartialOrganisations
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using EA.Weee.Core.Shared;
    using EA.Weee.RequestHandlers.Organisations.FindMatchingOrganisations.DataAccess;
    using Requests.Organisations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using OrganisationType = Domain.Organisation.OrganisationType;

    public class FindMatchingPartialOrganisationsHandler :
        IRequestHandler<FindMatchingPartialOrganisations, OrganisationSearchDataResult>
    {
        private readonly IFindMatchingOrganisationsDataAccess dataAccess;
        private readonly IUserContext userContext;

        public FindMatchingPartialOrganisationsHandler(IFindMatchingOrganisationsDataAccess dataAccess, IUserContext userContext)
        {
            this.dataAccess = dataAccess;
            this.userContext = userContext;
        }

        public async Task<OrganisationSearchDataResult> HandleAsync(FindMatchingPartialOrganisations query)
        {
            Guard.ArgumentNotNullOrEmpty(() => query.CompanyName, query.CompanyName);

            var possibleOrganisations = await dataAccess.GetOrganisationsByPartialSearchAsync(query.CompanyName, userContext.UserId);

            var totalMatchingOrganisationsData = possibleOrganisations.Select(o =>
                new PublicOrganisationData
                {
                    Id = o.Id,
                    DisplayName = o.OrganisationType == OrganisationType.RegisteredCompany || o.OrganisationType == OrganisationType.SoleTraderOrIndividual ? o.Name : o.TradingName,
                    Address = new AddressData
                    {
                        Address1 = o.BusinessAddress.Address1,
                        Address2 = o.BusinessAddress.Address2,
                        TownOrCity = o.BusinessAddress.TownOrCity,
                        CountyOrRegion = o.BusinessAddress.CountyOrRegion,
                        Postcode = o.BusinessAddress.Postcode,
                        CountryId = o.BusinessAddress.Country.Id,
                        Telephone = o.BusinessAddress.Telephone,
                        Email = o.BusinessAddress.Email
                    },
                }).ToList();

            return new OrganisationSearchDataResult(
                totalMatchingOrganisationsData,
                totalMatchingOrganisationsData.Count);
        }
    }
}
