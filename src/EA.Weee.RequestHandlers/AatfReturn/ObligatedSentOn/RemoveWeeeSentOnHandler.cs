namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.DataAccess;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RemoveWeeeSentOnHandler : IRequestHandler<RemoveWeeeSentOn, bool>
    {
        private readonly WeeeContext context;
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess sentOnDataAccess;
        private readonly IGenericDataAccess genericDataAccess;

        public RemoveWeeeSentOnHandler(WeeeContext context, IWeeeAuthorization authorization, IWeeeSentOnDataAccess sentOnDataAccess, IGenericDataAccess genericDataAccess)
        {
            this.context = context;
            this.authorization = authorization;
            this.sentOnDataAccess = sentOnDataAccess;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<bool> HandleAsync(RemoveWeeeSentOn message)
        {
            authorization.EnsureCanAccessExternalArea();

            var weeeSentOn = await genericDataAccess.GetById<WeeeSentOn>(message.WeeeSentOnId);

            await sentOnDataAccess.UpdateWeeeSentOnAsRemoved(weeeSentOn);

            return true;
        }
    }
}
