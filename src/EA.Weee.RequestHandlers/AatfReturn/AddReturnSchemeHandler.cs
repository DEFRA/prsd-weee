namespace EA.Weee.RequestHandlers.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
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
        private readonly IReturnDataAccess returnDataAccess;
        private readonly ISchemeDataAccess schemeDataAccess;

        public AddReturnSchemeHandler(IWeeeAuthorization authorization, IReturnSchemeDataAccess returnSchemeDataAccess, IReturnDataAccess returnDataAccess, ISchemeDataAccess schemeDataAccess)
        {
            this.authorization = authorization;
            this.returnSchemeDataAccess = returnSchemeDataAccess;
            this.returnDataAccess = returnDataAccess;
            this.schemeDataAccess = schemeDataAccess;
        }

        public async Task<Guid> HandleAsync(AddReturnScheme message)
        {
            authorization.EnsureCanAccessExternalArea();

            var scheme = await schemeDataAccess.GetSchemeOrDefault(message.SchemeId);

            var @return = await returnDataAccess.GetById(message.ReturnId);
            var returnScheme = new ReturnScheme(scheme, @return);
            try
            {
                await returnSchemeDataAccess.Submit(returnScheme);
            }
            catch (Exception ex)
            {
            }

            return returnScheme.Id;
        }
    }
}
