namespace EA.Weee.Requests.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetPreviousQuarterSchemes : IRequest<List<Guid>>
    {
        public Guid OrganisationId { get; set; }
    }
}
