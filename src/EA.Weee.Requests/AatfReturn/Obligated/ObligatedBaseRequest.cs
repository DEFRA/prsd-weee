namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using System.Collections.Generic;

    public abstract class ObligatedBaseRequest : IRequest<bool>
    {
        public IList<ObligatedValue> CategoryValues { get; set; }
    }
}
