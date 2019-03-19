namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;

    public abstract class ObligatedBaseRequest : IRequest<bool>
    {
        public IList<ObligatedValue> CategoryValues { get; set; }
    }
}
