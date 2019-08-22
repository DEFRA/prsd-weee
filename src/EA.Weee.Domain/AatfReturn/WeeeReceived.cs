namespace EA.Weee.Domain.AatfReturn
{
    using EA.Prsd.Core;
    using Scheme;
    using System;
    using System.Collections.Generic;

    public class WeeeReceived : AatfEntity, IReturnOption
    {
        public Guid SchemeId { get; private set; }

        public virtual Scheme Scheme { get; private set; }

        public IList<WeeeReceivedAmount> WeeeReceivedAmounts { get; set; }

        public WeeeReceived(Guid schemeId, Guid aatfId, Guid returnId)
        {
            SchemeId = schemeId;
            AatfId = aatfId;
            ReturnId = returnId;
        }

        public WeeeReceived(Scheme scheme, Aatf aatf, Return @return)
        {
            Guard.ArgumentNotNull(() => scheme, scheme);
            Guard.ArgumentNotNull(() => aatf, aatf);
            Guard.ArgumentNotNull(() => @return, @return);

            Scheme = scheme;
            Aatf = aatf;
            Return = @return;
        }
        public WeeeReceived()
        {
        }
    }
}
