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
        private readonly WeeeContext context;

        public EditObligatedReceivedHandler(IWeeeAuthorization authorization,
            IGenericDataAccess genericDataAccess, 
            WeeeContext context)
        {
            this.authorization = authorization;
            this.genericDataAccess = genericDataAccess;
            this.context = context;
        }

        public async Task<bool> HandleAsync(EditObligatedReceived message)
        {
            authorization.EnsureCanAccessExternalArea();

            foreach (var obligatedValue in message.CategoryValues)
            {
                var value = await genericDataAccess.GetById<WeeeReceivedAmount>(obligatedValue.Id);

                value.HouseholdTonnage = obligatedValue.HouseholdTonnage;
                value.NonHouseholdTonnage = obligatedValue.NonHouseholdTonnage;
            }

            await context.SaveChangesAsync();

            return true;
        }
    }
}
