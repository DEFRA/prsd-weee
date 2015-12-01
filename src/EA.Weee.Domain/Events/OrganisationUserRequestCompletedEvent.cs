namespace EA.Weee.Domain.Events
{
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Organisation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OrganisationUserRequestCompletedEvent : IEvent
    {
        public OrganisationUser OrganisationUser { get; private set; }

        internal OrganisationUserRequestCompletedEvent(OrganisationUser organisationUser)
        {
            OrganisationUser = organisationUser;
        }
    }
}
