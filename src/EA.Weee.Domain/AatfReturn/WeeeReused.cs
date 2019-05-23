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

        public virtual IList<WeeeReusedAmount> WeeeReusedAmounts { get; set; }

        public virtual IList<WeeeReusedSite> WeeeReusedSites { get; set; }

        public WeeeReused(Guid aatfId, Guid returnId)
        {
            AatfId = aatfId;
            ReturnId = returnId;
        }

        public WeeeReused(Aatf aatf, Guid returnId)
        {
            Guard.ArgumentNotNull(() => aatf, aatf);

            Aatf = aatf;
            ReturnId = returnId;
        }

        public WeeeReused()
        {
        }
    }
}
