namespace EA.Weee.Domain.DataReturns
{
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;
    using Lookup;

    public class EeeOutputAmount : ReturnItem
    {
        public virtual RegisteredProducer RegisteredProducer { get; private set; }

        public virtual DataReturnVersion DataReturnVersion { get; private set; }

        protected EeeOutputAmount()
        {
        }

        public EeeOutputAmount(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, RegisteredProducer registeredProducer, DataReturnVersion dataReturnVersion) :
            base(obligationType, weeeCategory, tonnage)
        {
            Guard.ArgumentNotNull(() => registeredProducer, registeredProducer);
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            RegisteredProducer = registeredProducer;
            DataReturnVersion = dataReturnVersion;
        }
    }
}
