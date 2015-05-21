namespace EA.Weee.RequestHandlers.Users
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Prsd.Core.Mediator;
    using Requests.Organisations;
    using Requests.Registration.Users;
    using Requests.Shared;

    internal class UserByIdHandler : IRequestHandler<UserById, User>
    {
        private readonly IwsContext context;

        public UserByIdHandler(IwsContext context)
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
                PhoneNumber = u.PhoneNumber,
                Surname = u.Surname,
                Organisation = new OrganisationData()
                {
                    Id = u.Organisation.Id,
                    Name = u.Organisation.Name,
                    Address = new AddressData()
                    {
                        Address2 = u.Organisation.Address.Address2,
                        Building = u.Organisation.Address.Building,
                        CountryName = u.Organisation.Address.Country,
                        PostalCode = u.Organisation.Address.PostalCode,
                        StreetOrSuburb = u.Organisation.Address.Address1,
                        TownOrCity = u.Organisation.Address.TownOrCity
                    }
                }
            }).SingleOrDefaultAsync(u => u.Id == query.Id);
        }
    }
}