namespace EA.Weee.Web.ViewModels.Payment
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class PaymentRequestViewModel
    {
        [Required] 
        public decimal Amount { get; set; }
        
        public string Description { get; set; }
        public string Reference { get; set; }

        [JsonPropertyName("return_url")] 
        public string ReturnUrl { get; set; }
    }
}