namespace EA.Weee.Domain.DataReturns
{
    using EA.Prsd.Core.Domain;
    using Prsd.Core;

    public class AatfDeliveryLocation : Entity
    {
        public string AatfApprovalNumber { get; private set; }

        public string FacilityName { get; private set; }

        protected AatfDeliveryLocation()
        {
        }

        public AatfDeliveryLocation(string aatfApprovalNumber, string facilityName)
        {
            Guard.ArgumentNotNullOrEmpty(() => aatfApprovalNumber, aatfApprovalNumber);
            Guard.ArgumentNotNullOrEmpty(() => facilityName, facilityName);

            AatfApprovalNumber = aatfApprovalNumber;
            FacilityName = facilityName;
        }
    }
}
