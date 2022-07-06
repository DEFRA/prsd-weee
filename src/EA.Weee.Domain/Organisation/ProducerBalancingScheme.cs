namespace EA.Weee.Domain.Organisation
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using Prsd.Core.Domain;

    public class ProducerBalancingScheme
    {
        public string Lock { get; set; }

        public virtual Organisation Organisation { get; private set; }

        public ProducerBalancingScheme()
        {
        }
    }
}
