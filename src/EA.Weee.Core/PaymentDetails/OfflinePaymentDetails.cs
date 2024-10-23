namespace EA.Weee.Core.PaymentDetails
{
    using System;
    public class OfflinePaymentDetails
    {
        public string PaymentMethod { get; set; }

        public DateTime? PaymentRecievedDate { get; set; }

        public string PaymentDetailsDescription { get; set; }

        public string RegistrationNumber { get; set; }
    }
}
