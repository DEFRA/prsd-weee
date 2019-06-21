namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;

    internal class EditObligatedSentOnHandler : IRequestHandler<EditObligatedSentOn, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IObligatedSentOnDataAccess obligatedSentOnDataAccess;

        public EditObligatedSentOnHandler(IWeeeAuthorization authorization,
            IObligatedSentOnDataAccess obligatedSentOnDataAccess,
            IGenericDataAccess genericDataAccess)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.obligatedSentOnDataAccess = obligatedSentOnDataAccess;
        }

        public async Task<bool> HandleAsync(EditObligatedSentOn message)
        {
            authorization.EnsureCanAccessExternalArea();

            foreach (var obligatedValue in message.CategoryValues)
            {
                var value = await genericDataAccess.GetById<WeeeSentOnAmount>(obligatedValue.Id);

                await obligatedSentOnDataAccess.UpdateAmounts(value, obligatedValue.HouseholdTonnage, obligatedValue.NonHouseholdTonnage);
            }

            return true;
        }
    }
}
