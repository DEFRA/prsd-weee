namespace EA.Weee.Core.AatfReturn
{
    using System.Collections.Generic;

    public class AddressTonnageSummary
    {
        public List<SiteAddressData> AddressData { get; set; }

        public List<WeeeObligatedData> ObligatedData { get; set; }
    }
}
