namespace EA.Weee.Requests.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class AddReturnScheme : IRequest<List<Guid>>
    {
        public List<Guid> SchemeIds { get; set; }

        public Guid ReturnId { get; set; }
    }
}
