namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using Prsd.Core.Mediator;

    public class SubmitReturn : IRequest<bool>
    {
        public Guid ReturnId { get; private set; }

        public bool NilReturn { get; private set; }

        public SubmitReturn(Guid returnId, bool nilReturn = false)
        {
            ReturnId = returnId;
            NilReturn = nilReturn;
        }
    }
}
