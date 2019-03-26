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

    internal class GetAatfSiteHandler : IRequestHandler<GetAatfSite, AddressTonnageSummary>
    {
        private readonly IWeeeAuthorization authorization;
        private readonly IAatfSiteDataAccess aatfSiteDataAccess;
        private readonly IMap<AatfAddressObligatedAmount, AddressTonnageSummary> mapper;

        public GetAatfSiteHandler(IWeeeAuthorization authorization,
            IAatfSiteDataAccess aatfSiteDataAccess,
            IMap<AatfAddressObligatedAmount, AddressTonnageSummary> mapper)
        {
            this.authorization = authorization;
            this.aatfSiteDataAccess = aatfSiteDataAccess;
            this.mapper = mapper;
        }

        public async Task<AddressTonnageSummary> HandleAsync(GetAatfSite message)
        {
            authorization.EnsureCanAccessExternalArea();

            var addressData = await aatfSiteDataAccess.GetAddresses(message.AatfId, message.ReturnId);

            var returnObligatedReusedValues = await aatfSiteDataAccess.GetObligatedWeeeForReturnAndAatf(message.AatfId, message.ReturnId);

            var aatfAddressObligatedAmount = new AatfAddressObligatedAmount(addressData, returnObligatedReusedValues);

            var result = mapper.Map(aatfAddressObligatedAmount);

            return result;
        }
    }
}
