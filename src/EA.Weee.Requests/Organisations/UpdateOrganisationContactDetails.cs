namespace EA.Weee.Requests.Organisations
{
    using EA.Prsd.Core.Mediator;
    using EA.Weee.Core.Organisations;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UpdateOrganisationContactDetails : IRequest<bool>
    {
        public OrganisationData OrganisationData { get; private set; }

        public UpdateOrganisationContactDetails(OrganisationData organisationData)
        {
            OrganisationData = organisationData;
        }
    }
}
