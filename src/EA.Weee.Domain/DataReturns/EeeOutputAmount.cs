namespace EA.Weee.Domain.DataReturns
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Producer;
    public class EeeOutputAmount : Entity
    {
        public virtual RegisteredProducer RegisteredProducer { get; private set; }

        public ObligationType ObligationType { get; private set; }

        public virtual DataReturnVersion DataReturnVersion { get; private set; }

        public Category Category { get; private set; }

        public decimal Tonnage { get; private set; }

        protected EeeOutputAmount()
        {
        }

        public EeeOutputAmount(ObligationType obligationType, Category category, decimal tonnage, RegisteredProducer registeredProducer, DataReturnVersion dataReturnVersion)
        {
            Guard.ArgumentNotNull(() => registeredProducer, registeredProducer);
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            ObligationType = obligationType;
            Category = category;
            Tonnage = tonnage;
            RegisteredProducer = registeredProducer;
            DataReturnVersion = dataReturnVersion;
        }
    }
}
