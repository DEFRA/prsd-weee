namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Prsd.Core.Mediator;

    public class FetchNonObligatedWeeeForReturnRequest : IRequest<List<decimal?>>
    {
        public Guid ReturnId { get; set; }

        public bool Dcf { get; set; }

        public FetchNonObligatedWeeeForReturnRequest(Guid returnId, bool dcf)
        {
            ReturnId = returnId;
            Dcf = dcf;
        }
    }
}
