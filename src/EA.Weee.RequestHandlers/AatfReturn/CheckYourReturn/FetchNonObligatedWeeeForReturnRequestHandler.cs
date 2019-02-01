namespace EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.DataReturns.FetchDataReturnComplianceYearsForScheme;
    using Security;
    using Request = EA.Weee.Requests.AatfReturn.FetchNonObligatedWeeeForReturnRequest;

    public class FetchNonObligatedWeeeForReturnRequestHandler : IRequestHandler<Request, List<decimal?>>
    {
        private readonly IFetchNonObligatedWeeeForReturnDataAccess dataAccess;

        public FetchNonObligatedWeeeForReturnRequestHandler(
            IFetchNonObligatedWeeeForReturnDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<List<decimal?>> HandleAsync(Request message)
        {
            return await dataAccess.GetDataReturnComplianceYearsForScheme(message.ReturnId, message.Dcf);
        }
    }
}
