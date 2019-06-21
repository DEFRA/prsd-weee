namespace EA.Weee.Requests.Scheme
{
    using System;
    using Core.Scheme;
    using Prsd.Core.Mediator;

    public class GetSchemeByOrganisationId : IRequest<SchemeData>
    {
        public Guid OrganisationId { get; private set; }

        public GetSchemeByOrganisationId(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
