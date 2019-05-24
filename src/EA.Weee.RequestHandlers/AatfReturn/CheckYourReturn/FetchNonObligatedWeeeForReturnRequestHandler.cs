namespace EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using NonObligated;
    using Security;
    using Request = Requests.AatfReturn.NonObligated.FetchNonObligatedWeeeForReturnRequest;

    public class FetchNonObligatedWeeeForReturnRequestHandler : IRequestHandler<Request, List<decimal?>>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly INonObligatedDataAccess dataAccess;

        public FetchNonObligatedWeeeForReturnRequestHandler(
            INonObligatedDataAccess dataAccess, IWeeeAuthorization authDataAccess)
        {
            this.dataAccess = dataAccess;
            this.authorization = authDataAccess;
        }

        public async Task<List<decimal?>> HandleAsync(Request message)
        {
            authorization.EnsureCanAccessExternalArea();
            authorization.EnsureOrganisationAccess(message.OrganisationsId);

            return await dataAccess.FetchNonObligatedWeeeForReturn(message.ReturnId, message.Dcf);
        }
    }
}