﻿namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class AddObligatedReceivedHandler : IRequestHandler<AddObligatedReceived, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligatedReceivedDataAccess obligatedReceivedDataAccess;

        public AddObligatedReceivedHandler(IWeeeAuthorization authorization,
            IObligatedReceivedDataAccess obligatedReceivedDataAccess)
        {
            this.authorization = authorization;
            this.obligatedReceivedDataAccess = obligatedReceivedDataAccess;
        }

        public async Task<bool> HandleAsync(AddObligatedReceived message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfWeeeReceived = new WeeeReceived(
                message.SchemeId,
                message.AatfId,
                message.ReturnId);

            var aatfWeeeReceivedAmount = new List<WeeeReceivedAmount>();

            foreach (var categoryValue in message.CategoryValues)
            {
                aatfWeeeReceivedAmount.Add(new WeeeReceivedAmount(aatfWeeeReceived, categoryValue.CategoryId, categoryValue.FirstTonnage, categoryValue.SecondTonnage));
            }

            await obligatedReceivedDataAccess.Submit(aatfWeeeReceivedAmount);

            return true;
        }
    }
}
