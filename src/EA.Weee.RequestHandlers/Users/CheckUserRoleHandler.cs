namespace EA.Weee.RequestHandlers.Users
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.Users;
    using System.Threading.Tasks;

    public class CheckUserRoleHandler : IRequestHandler<CheckUserRole, bool>
    {
        private readonly IWeeeAuthorization authorization;
        public CheckUserRoleHandler(IWeeeAuthorization authorization)
        {
            this.authorization = authorization;
        }

        public async Task<bool> HandleAsync(CheckUserRole message)
        {
            await Task.CompletedTask;

            return authorization.CheckUserInRole(message.Role);
        }
    }
}
