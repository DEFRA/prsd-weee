namespace EA.Weee.Domain.DataReturns
{
    using Lookup;
    using Prsd.Core;
    public abstract class WeeeDeliveredAmount : ReturnItem
    { 
        public virtual DataReturnVersion DataReturnVersion { get; private set; }

        public WeeeDeliveredAmount(ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, DataReturnVersion dataReturnVersion) :
            base(obligationType, weeeCategory, tonnage)
        {
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            DataReturnVersion = dataReturnVersion;
        }

        protected WeeeDeliveredAmount()
        {
        }
    }
}
