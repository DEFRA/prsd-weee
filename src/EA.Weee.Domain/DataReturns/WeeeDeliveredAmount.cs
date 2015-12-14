namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Lookup;
    using Prsd.Core.Domain;

    public class WeeeDeliveredAmount : Entity
    {
        public virtual ObligationType ObligationType { get; private set; }

        public virtual DataReturnVersion DataReturnVersion { get; private set; }

        public virtual WeeeCategory WeeeCategory { get; private set; }

        public decimal Tonnage { get; private set; }

        public virtual AatfDeliveryLocation AtfDeliveryLocation { get; private set; }

        public virtual AeDeliveryLocation AeDeliveryLocation { get; private set; }
    }
}
