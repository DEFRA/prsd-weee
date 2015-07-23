namespace EA.Weee.Domain.Producer
{
    using System;
    using Prsd.Core.Domain;

    public class MigratedProducer : Entity
    {
        public string ProducerRegistrationNumber { get; private set; }

        public string ProducerName { get; private set; }

        protected MigratedProducer()
        {          
        }
    }
}
