namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedSentOn
{
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;
    using System.Threading.Tasks;

    public class GetSentOnOperatorSiteHandler : IRequestHandler<GetSentOnOperatorSite, AatfAddressData>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly ISentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess;
        private readonly IMap<AatfAddress, AatfAddressData> mapper;

        public GetSentOnOperatorSiteHandler(IWeeeAuthorization authorization,
            ISentOnAatfSiteDataAccess getSentOnAatfSiteDataAccess, IMap<AatfAddress, AatfAddressData> mapper)
        {
            this.authorization = authorization;
            this.getSentOnAatfSiteDataAccess = getSentOnAatfSiteDataAccess;
            this.mapper = mapper;
        }

        public async Task<AatfAddressData> HandleAsync(GetSentOnOperatorSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var aatfAddress = await getSentOnAatfSiteDataAccess.GetWeeeSentOnOperatorAddress(message.WeeeSentOnId);

            var addressData = new AatfAddressData();

            if (aatfAddress != null)
            {
                addressData = mapper.Map(aatfAddress);
            }

            return addressData;
        }
    }
}
