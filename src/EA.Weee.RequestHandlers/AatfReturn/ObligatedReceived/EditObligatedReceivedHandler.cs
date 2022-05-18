namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReceived
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    internal class EditObligatedReceivedHandler : IRequestHandler<EditObligatedReceived, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IObligatedReceivedDataAccess obligatedReceivedDataAccess;

        public EditObligatedReceivedHandler(IWeeeAuthorization authorization,
            IObligatedReceivedDataAccess obligatedReceivedDataAccess,
            IGenericDataAccess genericDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.obligatedReceivedDataAccess = obligatedReceivedDataAccess;
        }

        public async Task<bool> HandleAsync(EditObligatedReceived message)
        {
            authorization.EnsureCanAccessExternalArea();

            foreach (var obligatedValue in message.CategoryValues)
            {
                var value = await genericDataAccess.GetById<WeeeReceivedAmount>(obligatedValue.Id);

                await obligatedReceivedDataAccess.UpdateAmounts(value, obligatedValue.FirstTonnage, obligatedValue.SecondTonnage);
            }

            return true;
        }
    }
}
