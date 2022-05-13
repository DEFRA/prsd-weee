namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess.DataAccess;

    public class AddObligatedSentOnHandler : IRequestHandler<AddObligatedSentOn, bool>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IObligatedSentOnDataAccess obligatedSentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;

        public AddObligatedSentOnHandler(IWeeeAuthorization authorization,
            IObligatedSentOnDataAccess obligatedSentOnDataAccess,
            IGenericDataAccess genericDataAccess)
        {
            this.authorization = authorization;
            this.obligatedSentOnDataAccess = obligatedSentOnDataAccess;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<bool> HandleAsync(AddObligatedSentOn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var weeeSent = await genericDataAccess.GetById<WeeeSentOn>(message.WeeeSentOnId);
            var aatfWeeeSentOnAmount = new List<WeeeSentOnAmount>();

            foreach (var categoryValue in message.CategoryValues)
            {
                aatfWeeeSentOnAmount.Add(new WeeeSentOnAmount(weeeSent, categoryValue.CategoryId, categoryValue.FirstTonnage, categoryValue.SecondTonnage));
            }

            await obligatedSentOnDataAccess.Submit(aatfWeeeSentOnAmount);

            return true;
        }
    }
}
