namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PaymentDetailsViewModel
    {
        public string RegistrationNumber { get; set; }

        [Required]
        [DisplayName("Payment method")]
        public string PaymentMethod { get; set; }

        [Required]
        [DisplayName("Payment recieved date")]
        public UKGOVDate PaymentRecievedDate { get; set; }

        [DisplayName("Payment details")]
        public string PaymentDetailsDescription { get; set; }

        [DisplayName("Confirm this payment has been made")]
        public bool ConfirmPaymentMade { get; set; }
    }
}
