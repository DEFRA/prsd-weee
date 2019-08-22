namespace EA.Weee.Requests.Admin
{
    using Core.Scheme;
    using Prsd.Core.Mediator;
    using System;
    using System.Collections.Generic;

    public class GetSchemesByOrganisationId : IRequest<List<SchemeData>>
    {
        public Guid OrganisationId { get; set; }

        public GetSchemesByOrganisationId(Guid id)
        {
            OrganisationId = id;
        }
    }
}
