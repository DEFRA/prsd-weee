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

    public class AatfDeliveryLocation : Entity, IReturnItem
    {
        public string AatfApprovalNumber { get; private set; }

        public string FacilityName { get; private set; }

        public virtual ObligationType ObligationType { get; private set; }

        public virtual WeeeCategory WeeeCategory { get; private set; }

        public decimal Tonnage { get; private set; }

        public virtual DataReturnVersion DataReturnVersion { get; private set; }

        protected AatfDeliveryLocation()
        {
        }

        public AatfDeliveryLocation(string aatfApprovalNumber, string facilityName, ObligationType obligationType, WeeeCategory weeeCategory, decimal tonnage, DataReturnVersion dataReturnVersion)
        {
            Guard.ArgumentNotNullOrEmpty(() => aatfApprovalNumber, aatfApprovalNumber);
            Guard.ArgumentNotNullOrEmpty(() => facilityName, facilityName);
            Guard.ArgumentNotNull(() => dataReturnVersion, dataReturnVersion);

            AatfApprovalNumber = aatfApprovalNumber;
            FacilityName = facilityName;
            ObligationType = obligationType;
            WeeeCategory = weeeCategory;
            Tonnage = tonnage;
            DataReturnVersion = dataReturnVersion;
        }
    }
}
