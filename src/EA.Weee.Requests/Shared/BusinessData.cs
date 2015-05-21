namespace EA.Weee.Requests.Shared
{
    using System.ComponentModel.DataAnnotations;
    using Prsd.Core.Validation;

    public class BusinessData
    {
        [Required]
        [Display(Name = "Organisation name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Organisation type")]
        public string EntityType { get; set; }

        public string RegistrationNumber
        {
            get
            {
                return CompaniesHouseRegistrationNumber ?? SoleTraderRegistrationNumber ?? PartnershipRegistrationNumber;
            }
        }

        [RequiredIf("EntityType", "Limited Company", "Companies house number is required")]
        [Display(Name = "Companies House number")]
        public string CompaniesHouseRegistrationNumber { get; set; }

        [RequiredIf("EntityType", "Sole Trader", "Sole Trader registration number is required")]
        [Display(Name = "Registration number")]
        public string SoleTraderRegistrationNumber { get; set; }

        [RequiredIf("EntityType", "Partnership", "Partnership registration number is required")]
        [Display(Name = "Registration number")]
        public string PartnershipRegistrationNumber { get; set; }

        [Display(Name = "Additional registration number")]
        public string AdditionalRegistrationNumber { get; set; }

        public void BindRegistrationNumber(string registrationNumber)
        {
            switch (this.EntityType)
            {
                case ("Sole Trader"):
                    this.SoleTraderRegistrationNumber = registrationNumber;
                    break;
                case ("Partnership"):
                    this.PartnershipRegistrationNumber = registrationNumber;
                    break;
                default:
                    this.CompaniesHouseRegistrationNumber = registrationNumber;
                    break;
            }
        }
    }
}
