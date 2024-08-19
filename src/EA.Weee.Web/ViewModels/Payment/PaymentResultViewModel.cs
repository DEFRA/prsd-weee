namespace EA.Weee.Web.ViewModels.Payment
{
    using System;
    public class PaymentResultViewModel
    {
        public long Amount { get; set; }
        public string Reference { get; set; }
        public string Email { get; set; }
        public string Card_brand { get; set; }
        public DateTime Created_date { get; set; }
        public string Description { get; set; }
        public long Total_amount { get; set; }
        public string Status { get; set; }
    }
}