namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Requests.Shared;

    internal class OrganisationByIdHandler : IRequestHandler<OrganisationById, OrganisationData>
    {
        private readonly WeeeContext context;

        public OrganisationByIdHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<OrganisationData> HandleAsync(OrganisationById query)
        {
            return await context
                .Organisations
                .Select(o => new OrganisationData
                {
                    Address = new AddressData
                    {
                        Address2 = o.OrganisationAddress.Address2,
                        Building = o.OrganisationAddress.Building,
                        CountryName = o.OrganisationAddress.Country,
                        PostalCode = o.OrganisationAddress.PostalCode,
                        StreetOrSuburb = o.OrganisationAddress.Address1,
                        TownOrCity = o.OrganisationAddress.TownOrCity
                    },
                    Id = o.Id,
                    Name = o.Name
                }).SingleAsync();
        }
    }
}