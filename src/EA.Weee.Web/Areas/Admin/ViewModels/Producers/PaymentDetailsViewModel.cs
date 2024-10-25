namespace EA.Weee.Web.Areas.Admin.ViewModels.Producers
{
    using EA.Weee.Core.PaymentDetails;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class PaymentDetailsViewModel
    {
        public Guid DirectProducerSubmissionId { get; set; }
        public string RegistrationNumber { get; set; }
        public int? Year { get; set; }

        [Required]
        [DisplayName("Payment method")]
        public string PaymentMethod { get; set; }

        [Required]
        [DisplayName("Payment received date")]
        public DateTimeInput PaymentReceivedDate { get; set; }

        [DisplayName("Payment details")]
        public string PaymentDetailsDescription { get; set; }

        [DisplayName("Confirm this payment has been made")]
        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Confirm this payment has been made is required")]
        public bool ConfirmPaymentMade { get; set; }
    }
}
