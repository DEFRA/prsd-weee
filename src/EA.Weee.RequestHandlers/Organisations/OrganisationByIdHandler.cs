namespace EA.Weee.RequestHandlers.Organisations
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Requests.Shared;

    internal class OrganisationByIdHandler : IRequestHandler<GetOrganisationInfo, OrganisationData>
    {
        private readonly WeeeContext context;

        public OrganisationByIdHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<OrganisationData> HandleAsync(GetOrganisationInfo query)
        {
            return await context
                .Organisations
                .Select(o => new OrganisationData
                {
                    OrganisationAddress = new AddressData
                    {
                        Address1 = o.OrganisationAddress.Address1,
                        Address2 = o.OrganisationAddress.Address2,
                        TownOrCity = o.OrganisationAddress.TownOrCity,
                        CountyOrRegion = o.OrganisationAddress.CountyOrRegion,
                        Postcode = o.OrganisationAddress.Postcode,
                        Country = o.OrganisationAddress.Country,
                        Telephone = o.OrganisationAddress.Telephone,
                        Email = o.OrganisationAddress.Email
                    },
                    Id = o.Id,
                    Name = o.Name
                }).SingleAsync(p => p.Id == query.OrganisationId);
        }
    }
}