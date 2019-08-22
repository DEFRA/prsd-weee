namespace EA.Weee.Requests.AatfReturn
{
    using Core.AatfReturn;
    using Prsd.Core.Mediator;
    using System;

    public class GetReturnStatus : IRequest<ReturnStatusData>
    {
        public Guid ReturnId { get; private set; }

        public GetReturnStatus(Guid returnId)
        {
            this.ReturnId = returnId;
        }
    }
}
