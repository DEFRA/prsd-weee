namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;

    public class ContactDetailsViewModel
    {
        [Required]
        [StringLength(CommonMaxFieldLengths.FirstName)]
        [Display(Name = "First name")]
        public virtual string FirstName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.LastName)]
        [Display(Name = "Last name")]
        public virtual string LastName { get; set; }

        [StringLength(CommonMaxFieldLengths.Position)]
        [Display(Name = "Position")]
        public virtual string Position { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.EmailAddress)]
        [Display(Name = "Email")]
        public virtual string Email { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Telephone)]
        [Display(Name = "Telephone")]
        public virtual string Telephone { get; set; }

        public Shared.AddressData AddressData { get; set; } = new Shared.AddressData();
    }
}