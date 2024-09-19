namespace EA.Weee.Domain.Producer
{
    using EA.Prsd.Core.Domain;

    public class PaymentState : Enumeration
    {
        public static readonly PaymentState New = new PaymentState(1, "New");
        public static readonly PaymentState Created = new PaymentState(2, "Created");
        public static readonly PaymentState Started = new PaymentState(3, "Started");
        public static readonly PaymentState Submitted = new PaymentState(4, "Submitted");
        public static readonly PaymentState Capturable = new PaymentState(5, "Capturable");
        public static readonly PaymentState Success = new PaymentState(6, "Success");
        public static readonly PaymentState Failed = new PaymentState(7, "Failed");
        public static readonly PaymentState Cancelled = new PaymentState(8, "Cancelled");
        public static readonly PaymentState Error = new PaymentState(9, "Error");

        protected PaymentState()
        {
        }

        private PaymentState(int value, string displayName)
            : base(value, displayName)
        {
        }
    }
}
