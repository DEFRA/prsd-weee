namespace EA.Weee.Domain.DataReturns
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EA.Prsd.Core.Domain;
    using Lookup;
    using Prsd.Core;

    public class AeDeliveryLocation : Entity, IReturnItem
    {
        public string ApprovalNumber { get; private set; }

        public string OperatorName { get; private set; }

        public virtual ObligationType ObligationType { get; private set; }

        public virtual WeeeCategory WeeeCategory { get; private set; }

        public decimal Tonnage { get; private set; }

        public virtual DataReturnVersion DataReturnVersion { get; private set; }

        protected AeDeliveryLocation()
        {
        }

        public AeDeliveryLocation(string approvalNumber, string operatorName, ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, DataReturnVersion dataReturnVersion)
        {
            Guard.ArgumentNotNullOrEmpty(() => approvalNumber, approvalNumber);
            Guard.ArgumentNotNullOrEmpty(() => operatorName, operatorName);
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            ApprovalNumber = approvalNumber;
            OperatorName = operatorName;
            ObligationType = obligationType;
            WeeeCategory = weeeCategory;
            Tonnage = tonnage;
            DataReturnVersion = dataReturnVersion;
        }
    }
}
