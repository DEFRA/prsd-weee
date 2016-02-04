namespace EA.Weee.Domain.DataReturns
{
    using System;
    using EA.Prsd.Core.Domain;
    using Prsd.Core;

    public class AatfDeliveryLocation : Entity, IEquatable<AatfDeliveryLocation>
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

        public bool Equals(AatfDeliveryLocation other)
        {
            if (other == null)
            {
                return false;
            }

            return ApprovalNumber == other.ApprovalNumber &&
                   FacilityName == other.FacilityName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AatfDeliveryLocation);
        }

        public override int GetHashCode()
        {
            return ApprovalNumber.GetHashCode() ^ FacilityName.GetHashCode();
        }
    }
}
