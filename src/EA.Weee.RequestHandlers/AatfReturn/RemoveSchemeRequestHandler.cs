namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System.Threading.Tasks;

    public class RemoveSchemeRequestHandler : IRequestHandler<RemoveScheme, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;

        public RemoveSchemeRequestHandler(IWeeeAuthorization authorization, IReturnSchemeDataAccess returnSchemeDataAccess)
        {
            this.authorization = authorization;
            this.returnSchemeDataAccess = returnSchemeDataAccess;
        }

        public async Task<bool> HandleAsync(RemoveScheme message)
        {
            authorization.EnsureCanAccessExternalArea();

            // Remove scheme

            // Remove weee data with that scheme id

            return true;
        }
    }
}
