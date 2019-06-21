namespace EA.Weee.RequestHandlers.AatfReturn.NonObligated
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.CheckYourReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.NonObligated;
    using System.Threading.Tasks;

    internal class EditNonObligatedHandler : IRequestHandler<EditNonObligated, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly INonObligatedDataAccess dataAccess;

        public EditNonObligatedHandler(IWeeeAuthorization authorization, INonObligatedDataAccess dataAccess, IGenericDataAccess genericDataAccess)
        {
            this.authorization = authorization;
            this.dataAccess = dataAccess;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<bool> HandleAsync(EditNonObligated message)
        {
            authorization.EnsureCanAccessExternalArea();

            foreach (var nonObligatedValue in message.CategoryValues)
            {
                var value = await genericDataAccess.GetById<NonObligatedWeee>(nonObligatedValue.Id);

                await dataAccess.UpdateAmount(value, nonObligatedValue.Tonnage);
            }

            return true;
        }
    }
}
