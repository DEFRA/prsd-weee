namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;

    public partial class Operator : Entity
    {
        public Operator(Guid operatorid, Guid organisationid)
        {
            OperatorId = operatorid;
            OrganisationId = organisationid;
        }

        public Guid OperatorId { get; private set; }

        public Guid OrganisationId { get; private set; }
    }
}
