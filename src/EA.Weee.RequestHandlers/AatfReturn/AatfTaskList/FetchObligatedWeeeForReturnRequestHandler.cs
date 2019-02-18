namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using Request = Requests.AatfReturn.ObligatedReceived.FetchObligatedWeeeForReturnRequest;

    public class FetchObligatedWeeeForReturnRequestHandler
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchObligatedWeeeForReturnDataAccess dataAccess;

        public FetchObligatedWeeeForReturnRequestHandler(
            IFetchObligatedWeeeForReturnDataAccess dataAccess, IWeeeAuthorization authdataaccess)
        {
            this.dataAccess = dataAccess;
            this.authorization = authdataaccess;
        }
    }
}
