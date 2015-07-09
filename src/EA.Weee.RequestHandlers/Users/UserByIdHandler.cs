namespace EA.Weee.RequestHandlers.Users
{
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.NewUser;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using Requests.NewUser;

    internal class UserByIdHandler : IRequestHandler<UserById, UserData>
    {
        private readonly WeeeContext context;

        public UserByIdHandler(WeeeContext context)
        {
            this.context = context;
        }

        public async Task<UserData> HandleAsync(UserById query)
        {
            return await context.Users.Select(u => new UserData
            {
                Email = u.Email,
                FirstName = u.FirstName,
                Id = u.Id,
                Surname = u.Surname,
            }).SingleOrDefaultAsync(u => u.Id == query.Id);
        }
    }
}