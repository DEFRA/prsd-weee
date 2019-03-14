namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetSentOnAatfSiteHandler : IRequestHandler<GetSentOnAatfSite, AddressData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAddSentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess;
        private readonly IMap<AatfAddress, AddressData> mapper;

        public GetSentOnAatfSiteHandler(IWeeeAuthorization authorization,
            IAddSentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess)
        {
            this.authorization = authorization;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
        }

        public async Task<AddressData> HandleAsync(GetSentOnAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfAddress = await getSentOnAatfSiteDataAccess.GetWeeeSentOnAddress(message.WeeeSentOnId);

            var addressData = mapper.Map(aatfAddress);

            return addressData;
        }
    }
}
