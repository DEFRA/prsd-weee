namespace EA.Weee.RequestHandlers.AatfReturn.Obligated
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.NonObligated;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.ObligatedReceived;

    internal class AddObligatedReceivedHandler : IRequestHandler<AddObligatedReceived, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddObligatedReceivedDataAccess obligatedReceivedDataAccess;
        private readonly IReturnDataAccess returnDataAccess;

        public AddObligatedReceivedHandler(IWeeeAuthorization authorization,
            IAddObligatedReceivedDataAccess obligatedReceivedDataAccess,
            IReturnDataAccess returnDataAccess)
        {
            this.authorization = authorization;
            this.obligatedReceivedDataAccess = obligatedReceivedDataAccess;
            this.returnDataAccess = returnDataAccess;
        }

        public async Task<bool> HandleAsync(AddObligatedReceived message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfWeeReceived = new WeeeReceived(
                await obligatedReceivedDataAccess.GetSchemeId(message.OrganisationId),
                await obligatedReceivedDataAccess.GetAatfId(message.OrganisationId),
                message.ReturnId);

            var aatfWeeReceivedAmount = new List<WeeeReceivedAmount>();

            foreach (var categoryValue in message.Tonnage)
            {
                aatfWeeReceivedAmount.Add(new WeeeReceivedAmount(aatfWeeReceived, categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage));
            }

            await obligatedReceivedDataAccess.Submit(aatfWeeReceivedAmount);

            return true;
        }
    }
}
