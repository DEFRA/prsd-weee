namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetReturn : IRequest<ReturnData>
    {
        public Guid ReturnId { get; private set; }

        public GetReturn(Guid returnId)
        {
            this.ReturnId = returnId;
        }
    }
}
