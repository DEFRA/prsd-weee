namespace EA.Weee.RequestHandlers.Users
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.NewUser;
    using Requests.Organisations;
    using Requests.Shared;

    internal class UserByIdHandler : IRequestHandler<UserById, User>
    {
        private readonly WeeeContext context;

        public UserByIdHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<User> HandleAsync(UserById query)
        {
            return await context.Users.Select(u => new User
            {
                Email = u.Email,
                FirstName = u.FirstName,
                Id = u.Id,
                Surname = u.Surname,
                Organisation = new OrganisationData()
                {
                    Id = u.Organisation.Id,
                    Name = u.Organisation.Name,
                    Address = new AddressData()
                    {
                        Address2 = u.Organisation.OrganisationAddress.Address2,
                        Building = u.Organisation.OrganisationAddress.Building,
                        CountryName = u.Organisation.OrganisationAddress.Country,
                        PostalCode = u.Organisation.OrganisationAddress.PostalCode,
                        StreetOrSuburb = u.Organisation.OrganisationAddress.Address1,
                        TownOrCity = u.Organisation.OrganisationAddress.TownOrCity
                    }
                }
            }).SingleOrDefaultAsync(u => u.Id == query.Id);
        }
    }
}