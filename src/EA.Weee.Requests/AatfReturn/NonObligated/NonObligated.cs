namespace EA.Weee.Requests.AatfReturn.NonObligated
{
    using EA.Prsd.Core.Mediator;
    using System.Collections.Generic;

    public abstract class NonObligated : IRequest<bool>
    {
        public IList<NonObligatedValue> CategoryValues { get; set; }
    }
}
