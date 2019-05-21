namespace EA.Weee.Requests.Admin
{
    using System;
    using System.Collections.Generic;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetSchemesByOrganisationId : IRequest<List<SchemeData>>
    {
        public Guid OrganisationId { get; set; }

        public GetSchemesByOrganisationId(Guid id)
        {
            OrganisationId = id;
        }
    }
}
