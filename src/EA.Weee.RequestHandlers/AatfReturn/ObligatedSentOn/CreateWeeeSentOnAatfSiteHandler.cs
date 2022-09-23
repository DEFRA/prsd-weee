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
        private readonly IGenericDataAccess genericDataAccess;

        public CreateWeeeSentOnAatfSiteHandler(IWeeeAuthorization authorization, IWeeeSentOnDataAccess sentOnDataAccess, IGenericDataAccess genericDataAccess)
        {
            this.authorization = authorization;
            this.sentOnDataAccess = sentOnDataAccess;
            this.genericDataAccess = genericDataAccess;
        }

        public async Task<Guid> HandleAsync(CreateWeeeSentOnAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();
            
            var aatfData = await genericDataAccess.GetById<Aatf>(message.SelectedAatfId);
            var weeeSentOn = new WeeeSentOn(message.ReturnId, message.AatfId, aatfData.Organisation.BusinessAddress.Id, aatfData.SiteAddress.Id);

            await sentOnDataAccess.Submit(weeeSentOn);

            return weeeSentOn.Id;
        }
    }
}
