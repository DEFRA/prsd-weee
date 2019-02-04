namespace EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess.DataAccess;
    using Security;
    using Request = Requests.AatfReturn.NonObligated.FetchNonObligatedWeeeForReturnRequest;

    public class FetchNonObligatedWeeeForReturnRequestHandler : IRequestHandler<Request, List<decimal?>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchNonObligatedWeeeForReturnDataAccess dataAccess;

        public FetchNonObligatedWeeeForReturnRequestHandler(
            IFetchNonObligatedWeeeForReturnDataAccess dataAccess, IWeeeAuthorization authdataaccess)
        {
            this.dataAccess = dataAccess;
            this.authorization = authdataaccess;
        }

        public async Task<List<decimal?>> HandleAsync(Request message)
        {
            authorization.EnsureCanAccessExternalArea();

            return await dataAccess.FetchNonObligatedWeeeForReturn(message.ReturnId, message.Dcf);
        }
    }
}
