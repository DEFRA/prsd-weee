namespace EA.Weee.Core.AatfReturn
{
    using EA.Weee.Core.DataStandards;
    using EA.Weee.Core.Shared;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Web.Mvc;

    [Serializable]
    public abstract class AddressData
    {
        public Guid Id { get; set; }

        public abstract string Name { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.AddressLine)]
        [Display(Name = "Address line 1")]
        public string Address1 { get; set; }

        [StringLength(CommonMaxFieldLengths.AddressLine)]
        [Display(Name = "Address line 2")]
        public string Address2 { get; set; }

        [Required]
        [StringLength(CommonMaxFieldLengths.TownCounty)]
        [Display(Name = "Town or city")]
        public string TownOrCity { get; set; }

        [StringLength(CommonMaxFieldLengths.TownCounty)]
        [Display(Name = "County or region")]
        public string CountyOrRegion { get; set; }

        [StringLength(CommonMaxFieldLengths.Postcode)]
        public string Postcode { get; set; }

        [Required]
        [Display(Name = "Country")]
        public Guid CountryId { get; set; }

        [Display(Name = "Country")]
        public string CountryName { get; set; }

        public IEnumerable<CountryData> Countries { get; set; }

        protected AddressData()
        {
        }

        protected AddressData(Guid id, string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId, string countryName)
        {
            Id = id;
            Name = name;
            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            CountyOrRegion = countyOrRegion;
            Postcode = postcode;
            CountryId = countryId;
            CountryName = countryName;
        }

        protected AddressData(string name, string address1, string address2, string townOrCity, string countyOrRegion, string postcode, Guid countryId, string countryName)
        {
            Name = name;
            Address1 = address1;
            Address2 = address2;
            TownOrCity = townOrCity;
            CountyOrRegion = countyOrRegion;
            Postcode = postcode;
            CountryId = countryId;
            CountryName = countryName;
        }

        public string ToAccessibleDisplayString(bool includeName, bool endLineWithComma = true)
        {
            var siteAddressStringBuilder = new StringBuilder();

            const string spanTagName = "span";
            var comma = endLineWithComma ? "," : string.Empty;

            var address1Span = new TagBuilder(spanTagName);

            if (includeName)
            {
                var addressNameSpan = new TagBuilder(spanTagName);
                addressNameSpan.SetInnerText($"{Name}{comma}");
                siteAddressStringBuilder.Append(addressNameSpan);
            }

            address1Span.SetInnerText($"{Address1}{comma}");
            siteAddressStringBuilder.Append(address1Span);

            if (Address2 != null)
            {
                var address2Span = new TagBuilder(spanTagName);
                address2Span.SetInnerText($"{Address2}{comma}");

                siteAddressStringBuilder.Append(address2Span);
            }

            var townOrCitySpan = new TagBuilder(spanTagName);
            townOrCitySpan.SetInnerText($"{TownOrCity}{comma}");
            siteAddressStringBuilder.Append(townOrCitySpan);

            if (CountyOrRegion != null)
            {
                var countySpan = new TagBuilder(spanTagName);
                countySpan.SetInnerText($"{CountyOrRegion}{comma}");
                siteAddressStringBuilder.Append(countySpan);
            }

            if (Postcode != null)
            {
                var postCodeSpan = new TagBuilder(spanTagName);
                postCodeSpan.SetInnerText($"{Postcode}{comma}");
                siteAddressStringBuilder.Append(postCodeSpan);
            }

            var countrySpan = new TagBuilder(spanTagName);
            countrySpan.SetInnerText(CountryName);
            siteAddressStringBuilder.Append(countrySpan);

            return siteAddressStringBuilder.ToString();
        }
    }
}
