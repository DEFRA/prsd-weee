namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System;
    using System.Threading.Tasks;

    public class CreateWeeeSentOnAatfSiteHandler : IRequestHandler<CreateWeeeSentOnAatfSite, Guid>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IWeeeSentOnDataAccess sentOnDataAccess;

        public CreateWeeeSentOnAatfSiteHandler(IWeeeAuthorization authorization, IWeeeSentOnDataAccess sentOnDataAccess)
        {
            this.authorization = authorization;
            this.sentOnDataAccess = sentOnDataAccess;
        }

        public async Task<Guid> HandleAsync(CreateWeeeSentOnAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();
            var weeeSentOn = await sentOnDataAccess.GetWeeeSentOnById(message.SelectedWeeeSentOnId);

            var createWeeeSentOnSite = new WeeeSentOn(message.ReturnId, message.AatfId, weeeSentOn.OperatorAddressId, weeeSentOn.SiteAddressId);
            await sentOnDataAccess.Submit(createWeeeSentOnSite);

            return createWeeeSentOnSite.Id;
        }
    }
}
