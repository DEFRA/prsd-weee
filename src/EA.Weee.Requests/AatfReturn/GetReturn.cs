namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetReturnStatus : IRequest<ReturnData>
    {
        public Guid ReturnId { get; private set; }

        public GetReturnStatus(Guid returnId)
        {
            this.ReturnId = returnId;
        }
    }
}
