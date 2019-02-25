namespace EA.Weee.Requests.AatfReturn.Obligated
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;

    public abstract class ObligatedReceived : IRequest<bool>
    {
        public IList<ObligatedValue> CategoryValues { get; set; }
    }
}
