namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core.Domain;

    public class PaymentStartedStatus : Enumeration
    {
        public static readonly PaymentStartedStatus Started = new PaymentStartedStatus(1, "Started");
        public static readonly PaymentStartedStatus Complete = new PaymentStartedStatus(2, "Complete");

        protected PaymentStartedStatus()
        {
        }

        private PaymentStartedStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
