namespace EA.Weee.Domain.AatfReturn
{
    using System;

    public class WeeeReusedSite
    {
        public virtual Guid WeeeReusedId { get; private set; }

        public virtual Guid AddressId { get; private set; }
    }
}
