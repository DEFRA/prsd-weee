namespace EA.Weee.Requests.Scheme
{
    using Core.Scheme;
    using Prsd.Core.Mediator;
    using System;

    public class GetSchemeByOrganisationId : IRequest<SchemeData>
    {
        public Guid OrganisationId { get; private set; }

        public GetSchemeByOrganisationId(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
