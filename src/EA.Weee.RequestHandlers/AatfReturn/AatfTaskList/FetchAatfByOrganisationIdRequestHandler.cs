namespace EA.Weee.RequestHandlers.AatfReturn.AatfTaskList
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.RequestHandlers.Security;

    public class FetchAatfByOrganisationIdRequestHandler
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IFetchObligatedWeeeForReturnDataAccess dataAccess;

        public FetchAatfByOrganisationIdRequestHandler(
            IFetchObligatedWeeeForReturnDataAccess dataAccess, IWeeeAuthorization authdataaccess)
        {
            this.dataAccess = dataAccess;
            this.authorization = authdataaccess;
        }
    }
}
