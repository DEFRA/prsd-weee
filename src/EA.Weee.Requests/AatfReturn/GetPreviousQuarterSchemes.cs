namespace EA.Weee.Requests.AatfReturn
{
    using EA.Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetPreviousQuarterSchemes : IRequest<List<Guid>>
    {
        public Guid OrganisationId { get; private set; }
        public Guid ReturnId { get; private set; }

        public GetPreviousQuarterSchemes(Guid organisationId, Guid returnId)
        {
            this.OrganisationId = organisationId;
            this.ReturnId = returnId;
        }
    }
}
