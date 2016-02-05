namespace EA.Weee.Requests.Organisations
{
    using System;
    using Core.Organisations;
    using Prsd.Core;
    using Prsd.Core.Mediator;

    public class OrganisationBySchemeId : IRequest<OrganisationData>
    {
        public Guid SchemeId { get; private set; }

        public OrganisationBySchemeId(Guid schemeId)
        {
            Guard.ArgumentNotDefaultValue(() => schemeId, schemeId);

            SchemeId = schemeId;
        }
    }
}
