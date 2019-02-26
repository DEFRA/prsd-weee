namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;

    internal class EditObligatedReceivedHandler : IRequestHandler<EditObligatedReceived, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IObligatedReceivedDataAccess obligatedReceivedDataAccess;
        private readonly WeeeContext context;

        public EditObligatedReceivedHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            WeeeContext context, 
            IObligatedReceivedDataAccess obligatedReceivedDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.context = context;
            this.obligatedReceivedDataAccess = obligatedReceivedDataAccess;
        }

        public async Task<bool> HandleAsync(EditObligatedReceived message)
        {
            authorization.EnsureCanAccessExternalArea();

            foreach (var obligatedValue in message.CategoryValues)
            {
                var value = await genericDataAccess.GetById<WeeeReceivedAmount>(obligatedValue.Id);

                await obligatedReceivedDataAccess.UpdateAmounts(value, obligatedValue.HouseholdTonnage, obligatedValue.NonHouseholdTonnage);
            }

            return true;
        }
    }
}
