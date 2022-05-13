namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    internal class EditObligatedReusedHandler : IRequestHandler<EditObligatedReused, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IObligatedReusedDataAccess obligatedReusedDataAccess;

        public EditObligatedReusedHandler(IWeeeAuthorization authorization,
            IObligatedReusedDataAccess obligatedReusedDataAccess,
            IGenericDataAccess genericDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.obligatedReusedDataAccess = obligatedReusedDataAccess;
        }

        public async Task<bool> HandleAsync(EditObligatedReused message)
        {
            authorization.EnsureCanAccessExternalArea();

            foreach (var obligatedValue in message.CategoryValues)
            {
                var value = await genericDataAccess.GetById<WeeeReusedAmount>(obligatedValue.Id);

                await obligatedReusedDataAccess.UpdateAmounts(value, obligatedValue.FirstTonnage, obligatedValue.SecondTonnage);
            }

            return true;
        }
    }
}
