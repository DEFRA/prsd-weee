namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using EA.Prsd.Core.Mediator;
    using System.Collections.Generic;
    using Aatf;

    public abstract class ObligatedBaseRequest : IRequest<bool>
    {
        public IList<TonnageValues> CategoryValues { get; set; }
    }
}
