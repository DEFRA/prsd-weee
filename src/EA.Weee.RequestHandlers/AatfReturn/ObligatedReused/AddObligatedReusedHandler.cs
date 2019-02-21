namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;

    internal class AddObligatedReusedHandler : IRequestHandler<AddObligatedReused, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddObligatedReusedDataAccess obligatedReusedDataAccess;

        public AddObligatedReusedHandler(IWeeeAuthorization authorization,
            IAddObligatedReusedDataAccess obligatedReusedDataAccess)
        {
            this.authorization = authorization;
            this.obligatedReusedDataAccess = obligatedReusedDataAccess;
        }

        public async Task<bool> HandleAsync(AddObligatedReused message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfWeeeReused = new WeeeReused(
                await obligatedReusedDataAccess.GetAatfId(message.OrganisationId),
                message.ReturnId);

            var aatfWeeeReusedAmount = new List<WeeeReusedAmount>();

            foreach (var categoryValue in message.CategoryValues)
            {
                aatfWeeeReusedAmount.Add(new WeeeReusedAmount(aatfWeeeReused, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
            }

            await obligatedReusedDataAccess.Submit(aatfWeeeReusedAmount);

            return true;
        }
    }
}
