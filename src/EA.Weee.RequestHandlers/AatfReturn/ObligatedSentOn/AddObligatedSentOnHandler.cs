namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AddObligatedSentOnHandler : IRequestHandler<AddObligatedSentOn, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligatedSentOnDataAccess obligatedSentOnDataAccess;

        public AddObligatedSentOnHandler(IWeeeAuthorization authorization,
            IObligatedSentOnDataAccess obligatedSentOnDataAccess)
        {
            this.authorization = authorization;
            this.obligatedSentOnDataAccess = obligatedSentOnDataAccess;
        }

        public async Task<bool> HandleAsync(AddObligatedSentOn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfWeeeSentOnAmount = new List<WeeeSentOnAmount>();

            foreach (var categoryValue in message.CategoryValues)
            {
                aatfWeeeSentOnAmount.Add(new WeeeSentOnAmount(categoryValue.CategoryId, categoryValue.HouseholdTonnage, categoryValue.NonHouseholdTonnage, message.WeeeSentOnId));
            }

            await obligatedSentOnDataAccess.Submit(aatfWeeeSentOnAmount);

            return true;
        }
    }
}
