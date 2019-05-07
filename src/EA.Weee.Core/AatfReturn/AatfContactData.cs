namespace EA.Weee.Core.AatfReturn
{
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;

    public class AatfContactData
    {
        [Required]
        [StringLength(CommonMaxFieldLengths.FirstName)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.LastName)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Position)]
        [Display(Name = "Position")]
        public string Position { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Telephone)]
        [Display(Name = "Telephone")]
        public string Telephone { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }
    }
}
