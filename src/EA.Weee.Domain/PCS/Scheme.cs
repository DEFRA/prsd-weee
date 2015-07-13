namespace EA.Weee.Domain.PCS
{
    using Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
    using Organisation;
    using Producer;

    public class Scheme : Entity
    {
        public virtual Organisation Organisation { get; private set; }

        public string ApprovalNumber { get; private set; }

        public virtual List<Producer> Producers { get; private set; }
    }
}
