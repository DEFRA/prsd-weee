namespace EA.Weee.Requests.AatfReturn
{
    using EA.Weee.Core.AatfReturn;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class NonObligatedRequest : IRequest
    {
        public IList<NonObligatedCategoryValue> CategoryValues { get; set; }

        public Guid NonObligatedId { get; set; }

        public Guid ReturnId { get; set; }
    }
}
