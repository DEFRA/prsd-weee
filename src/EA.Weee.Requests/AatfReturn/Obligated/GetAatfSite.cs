namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetAatfSite : IRequest<AddressTonnageSummary>
    {
        public Guid AatfId { get; private set; }

        public Guid ReturnId { get; private set; }

        public GetAatfSite(Guid aatfId, Guid returnId)
        {
            AatfId = aatfId;
            ReturnId = returnId;
        }
    }
}
