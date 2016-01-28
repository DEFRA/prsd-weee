namespace EA.Weee.Domain.DataReturns
{
    using System;
    using EA.Prsd.Core.Domain;
    using Prsd.Core;

    public class AeDeliveryLocation : Entity, IEquatable<AeDeliveryLocation>
    {
        public string ApprovalNumber { get; private set; }

        public string OperatorName { get; private set; }

        /// <summary>
        /// This constructor is used by Entity Framework.
        /// </summary>
        protected AeDeliveryLocation()
        {
        }

        public AeDeliveryLocation(string approvalNumber, string operatorName)
        {
            Guard.ArgumentNotNullOrEmpty(() => approvalNumber, approvalNumber);
            Guard.ArgumentNotNullOrEmpty(() => operatorName, operatorName);

            ApprovalNumber = approvalNumber;
            OperatorName = operatorName;
        }

        public bool Equals(AeDeliveryLocation other)
        {
            if (other == null)
            {
                return false;
            }

            return ApprovalNumber == other.ApprovalNumber &&
                   OperatorName == other.OperatorName;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AeDeliveryLocation);
        }

        public override int GetHashCode()
        {
            return ApprovalNumber.GetHashCode() ^ OperatorName.GetHashCode();
        }
    }
}
