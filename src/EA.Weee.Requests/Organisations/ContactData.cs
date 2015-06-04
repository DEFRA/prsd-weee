namespace EA.Weee.Requests.Organisations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ContactData
    {
        public Guid Id { get; set; }

        [StringLength(10)]
        [Display(Name = "Title (optional)")]
        [DataType(DataType.Text)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First name")]
        [DataType(DataType.Text)]
        public string Firstname { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last name")]
        [DataType(DataType.Text)]
        public string Lastname { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Position")]
        [DataType(DataType.Text)]
        public string Position { get; set; }
    }
}
