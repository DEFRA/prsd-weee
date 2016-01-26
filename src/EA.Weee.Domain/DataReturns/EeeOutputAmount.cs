namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using EA.Prsd.Core;
    using EA.Weee.Domain.Producer;
    using Lookup;
    using Obligation;

    public class EeeOutputAmount : ReturnItem, IEquatable<EeeOutputAmount>
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

        public bool Equals(EeeOutputAmount other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals((ReturnItem)other) &&
                   RegisteredProducer.Equals(other.RegisteredProducer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EeeOutputAmount);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ RegisteredProducer.GetHashCode();
        }
    }
}
