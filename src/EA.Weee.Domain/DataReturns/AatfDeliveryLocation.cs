﻿namespace EA.Weee.Domain.DataReturns
{
    using EA.Prsd.Core.Domain;
    using Prsd.Core;
    using System;

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
            int hashCode = ApprovalNumber.GetHashCode();

            if (!string.IsNullOrEmpty(FacilityName))
            {
                hashCode ^= FacilityName.GetHashCode();
            }

            return hashCode;
        }
    }
}
