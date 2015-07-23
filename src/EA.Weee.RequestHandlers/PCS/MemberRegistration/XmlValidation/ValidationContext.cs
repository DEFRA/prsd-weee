namespace EA.Weee.RequestHandlers.PCS.MemberRegistration.XmlValidation
{
    using System.Collections.Generic;
    using DataAccess;
    using Domain.Producer;

    public class ValidationContext : IValidationContext
    {
        private readonly WeeeContext context;

        public ValidationContext(WeeeContext context)
        {
            this.context = context;
        }

        public IEnumerable<Producer> Producers
        {
            get { return context.Producers; }
        }

        public IEnumerable<MigratedProducer> MigratedProducers
        {
            get { return context.MigratedProducers; }
        }
    }
}
