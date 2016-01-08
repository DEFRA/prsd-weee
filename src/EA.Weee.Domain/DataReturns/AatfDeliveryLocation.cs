namespace EA.Weee.Domain.DataReturns
{
    using EA.Prsd.Core.Domain;
    using Prsd.Core;

    public class AatfDeliveryLocation : Entity
    {
        public string ApprovalNumber { get; private set; }

        public string FacilityName { get; private set; }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected AatfDeliveryLocation()
        {
        }

        public AatfDeliveryLocation(string approvalNumber, string facilityName)
        {
            Guard.ArgumentNotNullOrEmpty(() => approvalNumber, approvalNumber);
            Guard.ArgumentNotNullOrEmpty(() => facilityName, facilityName);

            ApprovalNumber = approvalNumber;
            FacilityName = facilityName;
        }
    }
}
