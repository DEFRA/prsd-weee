namespace EA.Weee.Requests.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class RemoveReturnScheme : IRequest<bool>
    {
        public List<Guid> SchemeIds { get; set; }

        public Guid ReturnId { get; set; }
    }
}
