namespace EA.Weee.Requests.AatfReturn
{
    using Prsd.Core.Mediator;
    using System;

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
