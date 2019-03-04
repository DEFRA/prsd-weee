namespace EA.Weee.Requests.AatfReturn.NonObligated
{
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class NonObligated : IRequest<bool>
    {
        public IList<NonObligatedValue> CategoryValues { get; set; }
    }
}
