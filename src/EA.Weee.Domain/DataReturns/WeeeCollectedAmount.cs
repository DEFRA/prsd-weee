namespace EA.Weee.Domain.DataReturns
{
    using Lookup;
    using Prsd.Core;

    public class WeeeCollectedAmount : ReturnItem
    {
        public virtual WeeeCollectedAmountSourceType SourceType { get; private set; }

        public virtual DataReturnVersion DataReturnVersion { get; private set; }

        public WeeeCollectedAmount(WeeeCollectedAmountSourceType sourceType, ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, DataReturnVersion dataReturnVersion) :
            base(obligationType, weeeCategory, tonnage)
        {
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            SourceType = sourceType;
            DataReturnVersion = dataReturnVersion;
        }

        protected WeeeCollectedAmount()
        {
        }
    }
}
