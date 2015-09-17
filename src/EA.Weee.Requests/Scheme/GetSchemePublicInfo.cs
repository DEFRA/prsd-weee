namespace EA.Weee.Requests.Scheme
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class GetSchemePublicInfo : IRequest<SchemePublicInfo>
    {
        public Guid OrganisationId { get; private set; }

        public GetSchemePublicInfo(Guid organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
