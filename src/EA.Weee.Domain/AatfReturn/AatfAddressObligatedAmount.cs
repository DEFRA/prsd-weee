namespace EA.Weee.Domain.AatfReturn
{
    using System.Collections.Generic;
    public class AatfAddressObligatedAmount
    {
        public List<AatfAddress> AatfAddresses { get; private set; }

        public List<WeeeReusedAmount> WeeeReusedAmounts { get; private set; }

        public AatfAddressObligatedAmount(List<AatfAddress> aatfAddresses, List<WeeeReusedAmount> weeeReusedAmounts)
        {
            this.AatfAddresses = aatfAddresses;
            this.WeeeReusedAmounts = weeeReusedAmounts;
        }
    }
}
