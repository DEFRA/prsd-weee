namespace EA.Weee.Core.Organisations
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Core.DataStandards;
    public class SoleTraderViewModel
    {
        [Required]
        [DisplayName("First name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last name")]
        [StringLength(CommonMaxFieldLengths.DefaultString)]
        public string LastName { get; set; }
    }
}