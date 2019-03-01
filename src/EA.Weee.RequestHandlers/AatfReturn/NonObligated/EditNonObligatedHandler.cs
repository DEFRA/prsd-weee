namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditNonObligatedHandler : IRequestHandler<EditNonObligated, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly FetchNonObligatedWeeeForReturnDataAccess fetchDataAccess;
        private readonly NonObligatedDataAccess dataAccess;

        public EditNonObligatedHandler(IWeeeAuthorization authorization, NonObligatedDataAccess dataAccess, FetchNonObligatedWeeeForReturnDataAccess fetchDataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.fetchDataAccess = fetchDataAccess;
        }

        public async Task<bool> HandleAsync(EditNonObligated message)
        {
            authorization.EnsureCanAccessExternalArea();

            var values = fetchDataAccess.FetchNonObligatedWeeeForReturn(message.ReturnId);

            return true;
        }
    }
}
