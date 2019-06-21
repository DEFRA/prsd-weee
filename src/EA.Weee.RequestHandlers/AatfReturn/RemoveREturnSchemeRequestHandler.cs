namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System.Threading.Tasks;

    public class RemoveReturnSchemeRequestHandler : IRequestHandler<RemoveReturnScheme, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;

        public RemoveReturnSchemeRequestHandler(IWeeeAuthorization authorization, IReturnSchemeDataAccess returnSchemeDataAccess)
        {
            this.authorization = authorization;
            this.returnSchemeDataAccess = returnSchemeDataAccess;
        }

        public async Task<bool> HandleAsync(RemoveReturnScheme message)
        {
            authorization.EnsureCanAccessExternalArea();

            await returnSchemeDataAccess.RemoveReturnScheme(message.SchemeId);

            return true;
        }
    }
}
