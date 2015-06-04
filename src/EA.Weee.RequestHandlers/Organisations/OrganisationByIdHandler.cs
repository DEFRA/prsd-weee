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
                        Address1 = o.OrganisationAddress.Address1,
                        Address2 = o.OrganisationAddress.Address2,
                        TownOrCity = o.OrganisationAddress.TownOrCity,
                        CountyOrRegion = o.OrganisationAddress.CountyOrRegion,
                        PostalCode = o.OrganisationAddress.PostalCode,
                        Country = o.OrganisationAddress.Country,
                        Telphone = o.OrganisationAddress.TelePhone,
                        Email = o.OrganisationAddress.Email
                    },
                    Id = o.Id,
                    Name = o.Name
                }).SingleAsync();
        }
    }
}