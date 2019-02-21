namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;

    internal class AddObligatedReceivedHandler : IRequestHandler<AddObligatedReceived, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddObligatedReceivedDataAccess obligatedReceivedDataAccess;

        public AddObligatedReceivedHandler(IWeeeAuthorization authorization,
            IAddObligatedReceivedDataAccess obligatedReceivedDataAccess)
        {
            this.authorization = authorization;
            this.obligatedReceivedDataAccess = obligatedReceivedDataAccess;
        }

        public async Task<bool> HandleAsync(AddObligatedReceived message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfWeeReceived = new WeeeReceived(
                await obligatedReceivedDataAccess.GetSchemeId(message.OrganisationId),
                await obligatedReceivedDataAccess.GetAatfId(message.OrganisationId),
                message.ReturnId);

            var aatfWeeeReceivedAmount = new List<WeeeReceivedAmount>();

            foreach (var categoryValue in message.CategoryValues)
            {
                aatfWeeeReceivedAmount.Add(new WeeeReceivedAmount(aatfWeeReceived, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
            }

            await obligatedReceivedDataAccess.Submit(aatfWeeeReceivedAmount);

            return true;
        }
    }
}
