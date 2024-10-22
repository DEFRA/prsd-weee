using System;

namespace EA.Weee.Core.PaymentDetails
{
    public class OfflinePaymentDetails
    {
        public string PaymentMethod { get; set; }

        public DateTime PaymentRecievedDate { get; set; }

        public string PaymentDetailsDescription { get; set; }

     
        public bool ConfirmPaymentMade { get; set; }
    }
}
