namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Validation;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Web.Mvc;

    [Serializable]
    public class AatfContactData
    {
        public AatfContactData()
        {
            this.AddressData = new AatfContactAddressData();
        }

        public AatfContactData(Guid id, string firstName, string lastName, string position, AatfContactAddressData addressData, string telephone, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Position = position;
            AddressData = addressData;
            Telephone = telephone;
            Email = email;
        }

        public static string ToAccessibleDisplayString(AatfContactAddressData address, bool includeName)
        {
            if (address != null)
            {
                var siteAddressStringBuilder = new StringBuilder();

                const string spanTagName = "span";

                var address1Span = new TagBuilder(spanTagName);

                if (includeName)
                {
                    var addressNameSpan = new TagBuilder(spanTagName);
                    addressNameSpan.SetInnerText($"{address.Name},");
                    siteAddressStringBuilder.Append(addressNameSpan);
                }

                address1Span.SetInnerText($"{address.Address1},");
                siteAddressStringBuilder.Append(address1Span);

                if (address.Address2 != null)
                {
                    var address2Span = new TagBuilder(spanTagName);
                    address2Span.SetInnerText($"{address.Address2},");

                    siteAddressStringBuilder.Append(address2Span);
                }

                var townOrCitySpan = new TagBuilder(spanTagName);
                townOrCitySpan.SetInnerText($"{address.TownOrCity},");
                siteAddressStringBuilder.Append(townOrCitySpan);

                if (address.CountyOrRegion != null)
                {
                    var countySpan = new TagBuilder(spanTagName);
                    countySpan.SetInnerText($"{address.CountyOrRegion},");
                    siteAddressStringBuilder.Append(countySpan);
                }

                if (address.Postcode != null)
                {
                    var postCodeSpan = new TagBuilder(spanTagName);
                    postCodeSpan.SetInnerText($"{address.Postcode},");
                    siteAddressStringBuilder.Append(postCodeSpan);
                }

                var countrySpan = new TagBuilder(spanTagName);
                countrySpan.SetInnerText(address.CountryName);
                siteAddressStringBuilder.Append(countrySpan);

                return siteAddressStringBuilder.ToString();
            }

            return "&nbsp";
        }

        public Guid Id { get; set; }

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

        public AatfContactAddressData AddressData { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.Telephone)]
        [Display(Name = "Phone")]
        [GenericPhoneNumber(ErrorMessage = "The telephone number can use numbers, spaces and some special characters (-+). It must be no longer than 20 characters.")]
        public string Telephone { get; set; }

        [StringLength(CommonMaxFieldLengths.EmailAddress)]
        [EmailAddress(ErrorMessage = "The Email field is not a valid e-mail address")]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Enter email")]
        public string Email { get; set; }

        public bool CanEditContactDetails { get; set; }
    }
}
