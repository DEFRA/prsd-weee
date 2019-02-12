namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AddReturnSchemeHandler : IRequestHandler<AddReturnScheme, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IReturnSchemeDataAccess returnSchemeDataAccess;

        public AddReturnSchemeHandler(IWeeeAuthorization authorization, IReturnSchemeDataAccess returnSchemeDataAccess)
        {
            this.authorization = authorization;
            this.returnSchemeDataAccess = returnSchemeDataAccess;
        }

        public async Task<Guid> HandleAsync(AddReturnScheme message)
        {
            authorization.EnsureCanAccessExternalArea();

            var returnScheme = new ReturnScheme(message.SchemeId, message.ReturnId);

            await returnSchemeDataAccess.Submit(returnScheme);

            return returnScheme.Id;
        }
    }
}
