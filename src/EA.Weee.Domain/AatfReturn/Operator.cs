namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using Organisation;

    public partial class Operator : Entity
    {
        public Operator(Guid operatorid, Guid organisationid)
        {
            OrganisationId = organisationid;
        }

        public Guid OrganisationId { get; private set; }

        public Organisation Organisation { get; private set; }
    }
}
