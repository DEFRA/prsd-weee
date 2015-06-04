namespace EA.Weee.Requests.Organisations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ContactData
    {
        public Guid Id { get; set; }
    
        [Required]
        [StringLength(50)]
        [Display(Name = "First name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Position")]
        [DataType(DataType.Text)]
        public string Position { get; set; }
    }
}
