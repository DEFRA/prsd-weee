namespace EA.Weee.Requests.AatfReturn
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core.Mediator;

    public class RemoveReturnScheme : IRequest<bool>
    {
        public List<Guid> SchemeIds { get; set; }

        public Guid ReturnId { get; set; }
    }
}
