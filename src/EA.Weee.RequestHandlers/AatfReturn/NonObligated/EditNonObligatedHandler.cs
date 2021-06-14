namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.NonObligated;

    internal class EditNonObligatedHandler : IRequestHandler<EditNonObligated, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly INonObligatedDataAccess dataAccess;

        public EditNonObligatedHandler(IWeeeAuthorization authorization, INonObligatedDataAccess dataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
        }

        public async Task<bool> HandleAsync(EditNonObligated message)
        {
            authorization.EnsureCanAccessExternalArea();

            await dataAccess.UpdateAmountForIds(message.CategoryValues.Select(v => new Tuple<Guid, decimal?>(v.Id, v.Tonnage)));

            return true;
        }
    }
}
