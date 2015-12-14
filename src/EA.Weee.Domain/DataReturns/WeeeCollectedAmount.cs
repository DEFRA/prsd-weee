namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Lookup;
    using Prsd.Core;
    using Prsd.Core.Domain;

    public class WeeeCollectedAmount : Entity, IReturnItem
    {
        public virtual WeeeCollectedAmountSourceType SourceType { get; private set; }

        public virtual ObligationType ObligationType { get; private set; }

        public virtual WeeeCategory WeeeCategory { get; private set; }

        public decimal Tonnage { get; private set; }

        public virtual DataReturnVersion DataReturnVersion { get; private set; }

        public WeeeCollectedAmount(WeeeCollectedAmountSourceType sourceType, ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, DataReturnVersion dataReturnVersion)
        {
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            SourceType = sourceType;
            ObligationType = obligationType;
            WeeeCategory = weeeCategory;
            Tonnage = tonnage;
            DataReturnVersion = dataReturnVersion;
        }

        protected WeeeCollectedAmount()
        {
        }
    }
}
