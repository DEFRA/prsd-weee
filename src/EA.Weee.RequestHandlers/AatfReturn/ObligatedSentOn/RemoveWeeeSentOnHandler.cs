namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.AatfTaskList;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Threading.Tasks;

    public class RemoveWeeeSentOnHandler : IRequestHandler<RemoveWeeeSentOn, bool>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess sentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;
        private readonly IFetchObligatedWeeeForReturnDataAccess obligatedWeeeDataAccess;

        public RemoveWeeeSentOnHandler(WeeeContext context, IWeeeAuthorization authorization, IWeeeSentOnDataAccess sentOnDataAccess, IGenericDataAccess genericDataAccess, IFetchObligatedWeeeForReturnDataAccess obligatedWeeeDataAccess)
        {
            this.context = context;
            this.authorization = authorization;
            this.sentOnDataAccess = sentOnDataAccess;
            this.genericDataAccess = genericDataAccess;
            this.obligatedWeeeDataAccess = obligatedWeeeDataAccess;
        }

        public async Task<bool> HandleAsync(RemoveWeeeSentOn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var weeeSentOn = await genericDataAccess.GetById<WeeeSentOn>(message.WeeeSentOnId);

            if (weeeSentOn == null)
            {
                return false;
            }

            var weeeSentOnAmount = await obligatedWeeeDataAccess.FetchObligatedWeeeSentOnForReturn(message.WeeeSentOnId);

            var weeeSentOnListBySite = await sentOnDataAccess.GetWeeeSentOnBySiteId(weeeSentOn.SiteAddressId);
            if (weeeSentOnListBySite == 1)
            {
                genericDataAccess.Remove(weeeSentOn.SiteAddress);
            }

            var weeeSentOnListByOperator = await sentOnDataAccess.GetWeeeSentOnByOperatorId(weeeSentOn.OperatorAddressId);
            if (weeeSentOnListByOperator == 1)
            {
                genericDataAccess.Remove(weeeSentOn.OperatorAddress);
            }

            genericDataAccess.Remove(weeeSentOn);

            genericDataAccess.RemoveMany(weeeSentOnAmount);

            await context.SaveChangesAsync();

            return true;
        }
    }
}
