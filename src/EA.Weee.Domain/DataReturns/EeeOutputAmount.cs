namespace EA.Weee.Domain.DataReturns
{
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;
    using Lookup;

    public class EeeOutputAmount : ReturnItem
    {
        public virtual RegisteredProducer RegisteredProducer { get; private set; }

        public virtual ICollection<EeeOutputReturnVersion> EeeOutputReturnVersions { get; private set; }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected EeeOutputAmount()
        {
        }

        public EeeOutputAmount(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, RegisteredProducer registeredProducer) :
            base(obligationType, weeeCategory, tonnage)
        {
            Guard.ArgumentNotNull(() => registeredProducer, registeredProducer);

            RegisteredProducer = registeredProducer;
        }
    }
}
