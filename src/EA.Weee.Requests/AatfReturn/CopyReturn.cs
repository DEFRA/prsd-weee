namespace EA.Weee.Requests.AatfReturn
{
    using Prsd.Core.Mediator;
    using System;

    public class CopyReturn : IRequest<Guid>
    {
        public Guid ReturnId { get; private set; }

        public CopyReturn(Guid returnId)
        {
            this.ReturnId = returnId;
        }
    }
}
