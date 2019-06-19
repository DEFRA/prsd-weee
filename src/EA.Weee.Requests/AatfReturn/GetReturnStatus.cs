namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using Core.AatfReturn;
    using Prsd.Core.Mediator;

    public class GetReturn : IRequest<ReturnData>
    {
        public Guid ReturnId { get; private set; }

        public bool ForSummary { get; private set;  }

        public GetReturn(Guid returnId, bool forSummary)
        {
            this.ReturnId = returnId;
            this.ForSummary = forSummary;
        }
    }
}
