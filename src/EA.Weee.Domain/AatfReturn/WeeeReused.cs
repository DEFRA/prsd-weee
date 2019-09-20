namespace EA.Weee.Domain.AatfReturn
{
    using EA.Prsd.Core;
    using System;
    using System.Collections.Generic;

    public class WeeeReused : AatfEntity, IReturnOption
    {
        public IList<WeeeReusedAmount> WeeeReusedAmounts { get; set; }

        public IList<WeeeReusedSite> WeeeReusedSites { get; set; }

        public WeeeReused(Guid aatfId, Guid returnId)
        {
            AatfId = aatfId;
            ReturnId = returnId;
        }

        public WeeeReused(Aatf aatf, Return @return)
        {
            Guard.ArgumentNotNull(() => aatf, aatf);
            Guard.ArgumentNotNull(() => @return, @return);

            Aatf = aatf;
            Return = @return;
        }

        public WeeeReused()
        {
        }
    }
}
