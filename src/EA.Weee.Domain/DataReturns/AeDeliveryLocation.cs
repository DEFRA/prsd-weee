namespace EA.Weee.Domain.DataReturns
{
    using EA.Prsd.Core.Domain;
    using Prsd.Core;

    public class AeDeliveryLocation : Entity
    {
        public string ApprovalNumber { get; private set; }

        public string OperatorName { get; private set; }

        protected AeDeliveryLocation()
        {
        }

        public AeDeliveryLocation(string approvalNumber, string operatorName)
        {
            Guard.ArgumentNotNullOrEmpty(() => approvalNumber, approvalNumber);
            Guard.ArgumentNotNullOrEmpty(() => operatorName, operatorName);
        }
    }
}
