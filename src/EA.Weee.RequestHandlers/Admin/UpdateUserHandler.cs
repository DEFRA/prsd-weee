namespace EA.Weee.RequestHandlers.Admin
{
    using EA.Weee.DataAccess.Identity;
    using Microsoft.AspNet.Identity;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;
    using System;
    using System.Threading.Tasks;

    internal class UpdateUserHandler : IRequestHandler<UpdateUser, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly UserManager<ApplicationUser> userManager;

        public UpdateUserHandler(IWeeeAuthorization authorization, UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(UpdateUser query)
        {
            authorization.EnsureCanAccessInternalArea();

            var user = await userManager.FindByIdAsync(query.UserId);

            if (user == null)
            {
                throw new ArgumentException(string.Format("Could not find a user with id {0}",
                    query.UserId));
            }

            user.FirstName = query.FirstName;
            user.Surname = query.LastName;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                string message = "User update failed.";
                throw new Exception(message);
            }

            return new Guid(user.Id);
        }
    }
}
