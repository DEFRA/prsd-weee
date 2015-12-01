namespace EA.Weee.Core.Organisations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ContactData
    {
        public byte[] RowVersion { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "First name")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "Last name")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [StringLength(35)]
        [Display(Name = "Position")]
        [DataType(DataType.Text)]
        public string Position { get; set; }

        public bool HasContact { get; set; }

        public Guid OrganisationId { get; set; }
    }
}
