namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Requests.Shared;

    internal class OrganisationByIdHandler : IRequestHandler<OrganisationSearchById, OrganisationSearchData>
    {
        private readonly WeeeContext context;

        public OrganisationByIdHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<OrganisationSearchData> HandleAsync(OrganisationSearchById query)
        {
            return await context
                .Organisations
                .Select(o => new OrganisationSearchData
                {
                    Address = new AddressData
                    {
                        Address2 = o.Address.Address2,
                        Building = o.Address.Building,
                        CountryName = o.Address.Country,
                        PostalCode = o.Address.PostalCode,
                        StreetOrSuburb = o.Address.Address1,
                        TownOrCity = o.Address.TownOrCity
                    },
                    Id = o.Id,
                    Name = o.Name
                }).SingleAsync();
        }
    }
}