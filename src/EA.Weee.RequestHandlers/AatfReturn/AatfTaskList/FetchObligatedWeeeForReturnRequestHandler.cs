namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using EA.Weee.RequestHandlers.Security;

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
