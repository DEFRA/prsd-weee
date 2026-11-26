namespace EA.Weee.RequestHandlers.Admin
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Admin;
    using System;
    using System.Threading.Tasks;

    public class ValidateUserPasswordChangeDateHandler : IRequestHandler<ValidateUserPasswordChangeDate, bool>
    {
        private readonly IGetAdminUserDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        public ValidateUserPasswordChangeDateHandler(IGetAdminUserDataAccess dataAccess, IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.authorization = authorization;
        }

        public async Task<bool> HandleAsync(ValidateUserPasswordChangeDate message)
        {
            authorization.EnsureCanAccessInternalArea();

            var adminUser = await dataAccess.GetAdminUserOrDefault(new Guid(message.UserId));

            if (adminUser != null)
            {
                if (adminUser.User.PasswordChangedDate < DateTime.UtcNow.AddMonths(-6))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
