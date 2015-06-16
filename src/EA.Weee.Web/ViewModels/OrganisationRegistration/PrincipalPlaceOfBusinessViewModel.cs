namespace EA.Weee.Web.ViewModels.OrganisationRegistration
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using Validation;

    public class PrincipalPlaceOfBusinessViewModel
    {
        public PrincipalPlaceOfBusinessViewModel()
        {
            // TODO: Populate this from the database
            CountryOptions = new List<SelectListItem>
            {
                new SelectListItem { Text = "England", Value = "England"},
                new SelectListItem { Text = "Northern Ireland", Value = "NorthernIreland"},
                new SelectListItem { Text = "Scotland", Value = "Scotland"},
                new SelectListItem { Text = "Wales", Value = "Wales"},
            };
        }

        public IEnumerable<SelectListItem> CountryOptions { get; set; }

        public Guid OrganisationId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(35, ErrorMessage = "The Address line 1 field should be no more than 35 characters")]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [DataType(DataType.Text)]
        [StringLength(35, ErrorMessage = "The Address line 2 field should be no more than 35 characters")]
        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(35, ErrorMessage = "The Town or city field should be no more than 35 characters")]
        [Display(Name = "Town or city")]
        public string TownOrCity { get; set; }

        [DataType(DataType.Text)]
        [StringLength(35, ErrorMessage = "The County or region field should be no more than 35 characters")]
        [Display(Name = "County or region")]
        public string CountyOrRegion { get; set; }

        [StringLength(10, ErrorMessage = "The Postcode field should be no more than 10 characters")]
        [DataType(DataType.Text)]
        public string Postcode { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Country { get; set; } 

        [Required]
        [UkPhoneNumber(ErrorMessage = "The Phone field should be contain a valid UK phone number")]
        [StringLength(20, ErrorMessage = "The Phone field should be no more than 20 characters")]
        [Display(Name = "Phone")]
        [DataType(DataType.Text)]
        public string Telephone { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}