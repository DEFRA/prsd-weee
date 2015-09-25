namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Data.Entity;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;
    using System.Threading.Tasks;
    using DataAccess;

    internal class UpdateUserHandler : IRequestHandler<UpdateUser, Guid>
    {
        private readonly WeeeContext db;
        private readonly IWeeeAuthorization authorization;

        public UpdateUserHandler(WeeeContext context, IWeeeAuthorization authorization)
        {
            db = context;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(UpdateUser query)
        {
            authorization.EnsureCanAccessInternalArea();

            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == query.UserId);

            if (user == null)
            {
                throw new ArgumentException(string.Format("Could not find a user with id {0}",
                    query.UserId));
            }

            user.UpdateUserInfo(query.FirstName, query.LastName);

            await db.SaveChangesAsync();

            return new Guid(user.Id);
        }
    }
}
