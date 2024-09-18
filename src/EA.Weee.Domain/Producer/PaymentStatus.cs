namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core.Domain;

    public class PaymentStatus : Enumeration
    {
        public static readonly PaymentStatus Started = new PaymentStatus(1, "Started");
        public static readonly PaymentStatus Complete = new PaymentStatus(2, "Complete");

        protected PaymentStatus()
        {
        }

        private PaymentStatus(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
