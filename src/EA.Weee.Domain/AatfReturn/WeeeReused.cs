namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public class WeeeReused : ReturnEntity, IReturnOption
    {
        public virtual Guid AatfId { get; private set; }

        public virtual Aatf Aatf { get; private set; }

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
