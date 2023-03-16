namespace EA.Weee.Web.Areas.AatfReturn.ViewModels
{
    using EA.Weee.Core.AatfReturn;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using System.Web.Mvc;

    public class SentOnSiteSummaryListViewModel
    {
        public Guid OrganisationId { get; set; }

        public Guid ReturnId { get; set; }

        public Guid AatfId { get; set; }

        public String AatfName { get; set; }

        public List<WeeeSentOnSummaryListData> Sites { get; set; }

        public ObligatedCategoryValue Tonnages { get; set; }

        public bool IsChkCopyPreviousQuarterVisible { get; set; }

        [Required]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Please select copy selection from previous quarter")]
        public bool IsCopyChecked { get; set; }        

        public string CreateLongAddress(AatfAddressData address)
        {
            if (address != null)
            {
                var siteAddressStringBuilder = new StringBuilder();

                const string spanTagName = "span";
                var addressNameSpan = new TagBuilder(spanTagName);
                var address1Span = new TagBuilder(spanTagName);

                addressNameSpan.SetInnerText($"{address.Name},");
                address1Span.SetInnerText($"{address.Address1},");

                siteAddressStringBuilder.Append(addressNameSpan);
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
    }
}