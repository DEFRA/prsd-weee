namespace EA.Weee.Domain.DataReturns
{
    using System.Collections.Generic;
    using Lookup;
    using Prsd.Core;

    public class WeeeCollectedAmount : ReturnItem
    {
        public virtual WeeeCollectedAmountSourceType SourceType { get; private set; }

        public virtual ICollection<WeeeCollectedReturnVersion> WeeeCollectedReturnVersions { get; private set; }

        public WeeeCollectedAmount(WeeeCollectedAmountSourceType sourceType, ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage) :
            base(obligationType, weeeCategory, tonnage)
        {
            SourceType = sourceType;
            WeeeCollectedReturnVersions = new List<WeeeCollectedReturnVersion>();
        }

        protected WeeeCollectedAmount()
        {
        }

        public void AddWeeeCollectedReturnVersion(WeeeCollectedReturnVersion version)
        {
            Guard.ArgumentNotNull(() => version, version);
            WeeeCollectedReturnVersions.Add(version);
        }
    }
}
