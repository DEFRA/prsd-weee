namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.Obligated;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;

    internal class AddObligatedReusedHandler
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddObligatedReusedDataAccess obligatedReusedDataAccess;

        public AddObligatedReusedHandler(IWeeeAuthorization authorization,
            IAddObligatedReusedDataAccess obligatedReusedDataAccess)
        {
            this.authorization = authorization;
            this.obligatedReusedDataAccess = obligatedReusedDataAccess;
        }

        public async Task<bool> HandleAsync(AddObligated message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfWeeReceived = new WeeeReused(
                await obligatedReusedDataAccess.GetSchemeId(message.OrganisationId),
                await obligatedReusedDataAccess.GetAatfId(message.OrganisationId),
                message.ReturnId);

            var aatfWeeeReusedAmount = new List<WeeeReusedAmount>();

            foreach (var categoryValue in message.CategoryValues)
            {
                aatfWeeeReusedAmount.Add(new WeeeReusedAmount(aatfWeeReceived, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
            }

            await obligatedReusedDataAccess.Submit(aatfWeeeReusedAmount);

            return true;
        }
    }
}
