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

    internal class EditNonObligatedHandler : IRequestHandler<EditNonObligated, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly FetchNonObligatedWeeeForReturnDataAccess fetchDataAccess;
        private readonly NonObligatedDataAccess dataAccess;

        public EditNonObligatedHandler(IWeeeAuthorization authorization, NonObligatedDataAccess dataAccess, FetchNonObligatedWeeeForReturnDataAccess fetchDataAccess, IGenericDataAccess genericDataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.fetchDataAccess = fetchDataAccess;
            this.genericDataAccess = genericDataAccess;
    }

        public async Task<bool> HandleAsync(EditNonObligated message)
        {
            authorization.EnsureCanAccessExternalArea();

            foreach (var nonObligatedValue in message.CategoryValues)
            {
                var value = await genericDataAccess.GetById<NonObligatedWeee>(nonObligatedValue.Id);

                await dataAccess.UpdateAmount(value, nonObligatedValue.Tonnage);
                //await obligatedReceivedDataAccess.UpdateAmounts(value, obligatedValue.HouseholdTonnage, obligatedValue.NonHouseholdTonnage);
            }

            return true;
        }
    }
}
