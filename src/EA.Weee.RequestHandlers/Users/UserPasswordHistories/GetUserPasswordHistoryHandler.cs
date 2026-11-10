namespace EA.Weee.RequestHandlers.Users.UserPasswordHistories
{
    using EA.Weee.Requests.Users;
    using Prsd.Core.Mediator;
    using System.Threading.Tasks;

    public class GetUserPasswordHistoryHandler : IRequestHandler<GetUserPasswordHistory, bool>
    {
        private readonly IUserPasswordHistoriesDataAccess dataAccess;

        public GetUserPasswordHistoryHandler(IUserPasswordHistoriesDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(GetUserPasswordHistory query)
        {
            return await dataAccess.ValidateUserPasswordHistories(query);
        }
    }
}
