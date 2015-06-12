namespace EA.Weee.Web.ViewModels.OrganisationRegistration.Details
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using EA.Weee.Requests.Organisations;
    using EA.Weee.Requests.Shared;
    // Organisation contact details viewmodel
    public class OrganisationContactDetailsViewModel
    {
        public Guid OrganisationId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(50)]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Town or city")]
        public string TownOrCity { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "County or region")]
        public string CountyOrRegion { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Postcode")]
        public string Postcode { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Country")]
        public string Country { get; set; }

        [Required]
        [RegularExpression("^[0-9+\\(\\)#\\.\\s\\/ext-]+$", ErrorMessage = "The entered phone number is invalid")]
        [DataType(DataType.Text)]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        public AddOrganisationContactDetails ToAddRequest()
        {
            return new AddOrganisationContactDetails
            {
                OrganisationId = OrganisationId,
                OrganisationContactAddress = new AddressData()
                {
                    Address1 = Address1,
                    Address2 = Address2,
                    TownOrCity = TownOrCity,
                    CountyOrRegion = CountyOrRegion,
                    PostalCode = Postcode,
                    Country = Country,
                    Telephone = Phone,
                    Email = Email
                }
            };
        }
    }
}