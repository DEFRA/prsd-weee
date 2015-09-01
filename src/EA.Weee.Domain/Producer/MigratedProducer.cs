namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class MigratedProducer : Entity
    {
        public virtual string ProducerRegistrationNumber { get; private set; }

        public virtual string ProducerName { get; private set; }

        protected MigratedProducer()
        {          
        }
    }
}
