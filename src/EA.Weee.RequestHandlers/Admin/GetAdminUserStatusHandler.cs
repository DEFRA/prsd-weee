namespace EA.Weee.RequestHandlers.Admin
{
    using System;
    using System.Threading.Tasks;
    using Core.Users;
    using Domain;
    using Prsd.Core.Mapper;
    using Prsd.Core.Mediator;
    using Requests.Admin;
    using Security;

    public class GetAdminUserStatusHandler : IRequestHandler<GetAdminUserStatus, Core.Shared.UserStatus>
    {
        private readonly IGetAdminUserDataAccess dataAccess;
        private readonly IWeeeAuthorization authorization;

        private readonly IMap<UserStatus, Core.Shared.UserStatus> userMap;

        public GetAdminUserStatusHandler(IGetAdminUserDataAccess dataAccess, IMap<UserStatus, Core.Shared.UserStatus> userMap,
            IWeeeAuthorization authorization)
        {
            this.dataAccess = dataAccess;
            this.userMap = userMap;
            this.authorization = authorization;
        }

        public async Task<Core.Shared.UserStatus> HandleAsync(GetAdminUserStatus request)
        {
            authorization.EnsureCanAccessInternalArea(false);

            var adminUser = await dataAccess.GetAdminUserOrDefault(new Guid(request.UserId));

            if (adminUser == null)
            {
                string message = string.Format("No user was found with id \"{0}\".", request.UserId);
                throw new ArgumentException(message);
            }
            return userMap.Map(adminUser.UserStatus);
        }
    }
}
