namespace EA.Weee.RequestHandlers.AatfReturn.ObligatedReused
{
    using System.Threading.Tasks;
    using EA.Prsd.Core.Mapper;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;
    using EA.Weee.Domain.AatfReturn;
    using EA.Weee.RequestHandlers.AatfReturn.ObligatedGeneric;
    using EA.Weee.RequestHandlers.Security;
    using EA.Weee.Requests.AatfReturn.Obligated;

    internal class GetAatfSiteRequestHandler : IRequestHandler<GetAatfSite, AddressTonnageSummary>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IGetAatfSiteDataAccess getAatfSiteDataAccess;
        private readonly IMap<AatfAddressObligatedAmount, AddressTonnageSummary> mapper;

        public GetAatfSiteRequestHandler(IWeeeAuthorization authorization,
            IGetAatfSiteDataAccess getAatfSiteDataAccess,
            IMap<AatfAddressObligatedAmount, AddressTonnageSummary> mapper)
        {
            this.authorization = authorization;
            this.getAatfSiteDataAccess = getAatfSiteDataAccess;
            this.mapper = mapper;
        }

        public async Task<AddressTonnageSummary> HandleAsync(GetAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var addressData = await getAatfSiteDataAccess.GetAddresses(message.AatfId, message.ReturnId);

            var returnObligatedReusedValues = await getAatfSiteDataAccess.GetObligatedWeeeForReturnAndAatf(message.AatfId, message.ReturnId);

            var aatfAddressObligatedAmount = new AatfAddressObligatedAmount(addressData, returnObligatedReusedValues);

            var result = mapper.Map(aatfAddressObligatedAmount);

            return result;
        }
    }
}
