﻿namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal class AddObligatedReusedHandler : IRequestHandler<AddObligatedReused, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligatedReusedDataAccess obligatedReusedDataAccess;

        public AddObligatedReusedHandler(IWeeeAuthorization authorization,
            IObligatedReusedDataAccess obligatedReusedDataAccess)
        {
            this.authorization = authorization;
            this.obligatedReusedDataAccess = obligatedReusedDataAccess;
        }

        public async Task<bool> HandleAsync(AddObligatedReused message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfWeeeReused = new WeeeReused(
                message.AatfId,
                message.ReturnId);

            var aatfWeeeReusedAmount = new List<WeeeReusedAmount>();

            foreach (var categoryValue in message.CategoryValues)
            {
                aatfWeeeReusedAmount.Add(new WeeeReusedAmount(aatfWeeeReused, categoryValue.CategoryId, categoryValue.FirstTonnage, categoryValue.SecondTonnage));
            }

            await obligatedReusedDataAccess.Submit(aatfWeeeReusedAmount);

            return true;
        }
    }
}
