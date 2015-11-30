namespace EA.Weee.Requests.Scheme
{
    using System;
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Scheme;

    public class GetSchemePublicInfo : IRequest<SchemePublicInfo>
    {
        public Guid OrganisationId { get; private set; }

        public GetSchemePublicInfo(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
