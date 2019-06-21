namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.AatfReturn;

    public class AatfSite : IRequest<bool>
    {
        public SiteAddressData AddressData { get; set; }
    }
}
