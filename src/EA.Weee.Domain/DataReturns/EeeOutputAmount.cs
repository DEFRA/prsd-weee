namespace EA.Weee.Domain.DataReturns
{
    using EA.Prsd.Core;
    using EA.Prsd.Core.Domain;
    using EA.Weee.Domain.Lookup;
    using EA.Weee.Domain.Producer;

    public class EeeOutputAmount : Entity, IReturnItem
    {
        public virtual RegisteredProducer RegisteredProducer { get; private set; }

        public virtual ObligationType ObligationType { get; private set; }

        public virtual DataReturnVersion DataReturnVersion { get; private set; }

        public virtual WeeeCategory WeeeCategory { get; private set; }

        public decimal Tonnage { get; private set; }

        protected EeeOutputAmount()
        {
        }

        public EeeOutputAmount(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, RegisteredProducer registeredProducer, DataReturnVersion dataReturnVersion)
        {
            Guard.ArgumentNotNull(() => registeredProducer, registeredProducer);
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            ObligationType = obligationType;
            WeeeCategory = weeeCategory;
            Tonnage = tonnage;
            RegisteredProducer = registeredProducer;
            DataReturnVersion = dataReturnVersion;
        }
    }
}
