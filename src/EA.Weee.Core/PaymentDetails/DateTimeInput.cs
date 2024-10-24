namespace EA.Weee.Core.PaymentDetails
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class DateTimeInput
    {
        [Required]
        [Range(1, 31)]
        public int Day { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Range(1900, 2100)]
        public int Year { get; set; }

        public DateTime ToDateTime() => new DateTime(this.Year, this.Month, this.Day);
    }
}
