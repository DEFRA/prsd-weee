namespace EA.Weee.Domain.PCS
{
    using Prsd.Core.Domain;
    using System;
    using System.Collections.Generic;
    using Organisation;
    using Producer;

    public class Scheme : Entity
    {
        public Scheme(Guid organisationId)
        {
            OrganisationId = organisationId;
            ApprovalNumber = string.Empty;
            Producers = new List<Producer>();
        }

        protected Scheme()
        {
        }

        public virtual Guid OrganisationId { get; private set; }

        public virtual Organisation Organisation { get; private set; }

        public string ApprovalNumber { get; private set; }

        public virtual List<Producer> Producers { get; private set; }

        public void SetProducers(List<Producer> producers)
        {
            Producers = producers;
        }
    }
}
