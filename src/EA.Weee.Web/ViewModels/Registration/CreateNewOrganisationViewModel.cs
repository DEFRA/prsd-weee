namespace EA.Weee.Web.ViewModels.Registration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Prsd.Core.Validation;
    using Prsd.Core.Web;
    using Requests.Registration;

    public class CreateNewOrganisationViewModel
    {
        private const string DefaultCountryName = "United Kingdom";

        public IEnumerable<CountryData> Countries { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrganisationId { get; set; }

        [Required]
        [Display(Name = "Organisation name")]
        public string Name { get; set; }

        [RequiredIf("EntityType", "Limited Company", "Companies House number is required")]
        [Display(Name = "Companies House number")]
        public string CompaniesHouseReference { get; set; }

        [Required]
        [Display(Name = "Building name or number")]
        public string Building { get; set; }

        [Required]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required]
        [Display(Name = "Town or city")]
        public string TownOrCity { get; set; }

        [Required]
        public string Postcode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }

        [Required]
        [Display(Name = "Organisation type")]
        public string EntityType { get; set; }

        public CountryData DefaultCountry
        {
            get
            {
                if (Countries == null || !Countries.Any())
                {
                    return null;
                }

                var country = Countries.SingleOrDefault(c => c.Name.Equals(DefaultCountryName));

                if (this.CountryId == Guid.Empty)
                {
                    if (country != null)
                    {
                        this.CountryId = country.Id;
                    }
                }

                return country ?? Countries.First();
            }
        }
    }
}