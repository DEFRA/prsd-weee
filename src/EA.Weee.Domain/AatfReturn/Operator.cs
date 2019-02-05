namespace EA.Weee.Domain.AatfReturn
{
    using System;
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using Organisation;

    public partial class Operator : Entity
    {
        public Operator()
        {
        }

        public Operator(Organisation organisation)
        {
            Guard.ArgumentNotNull(() => organisation, organisation);

            Organisation = organisation;
        }

        public virtual Organisation Organisation { get; private set; }
    }
}
