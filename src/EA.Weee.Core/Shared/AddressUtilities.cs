namespace EA.Weee.Core.Shared
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;

    public class AddressUtilities : IAddressUtilities
    {
        public string AddressConcatenate(AatfReturn.AddressData addressData)
        {
            var address = addressData.Address1;

            address = addressData.Address2 == null ? address : StringConcatenate(address, addressData.Address2);

            address = StringConcatenate(address, addressData.TownOrCity);

            address = addressData.CountyOrRegion == null ? address : StringConcatenate(address, addressData.CountyOrRegion);

            address = addressData.Postcode == null ? address : StringConcatenate(address, addressData.Postcode);

            address = StringConcatenate(address, addressData.CountryName);

            return address;
        }

        public string StringConcatenate(string address, string input)
        {
            return $"{address}, {input}";
        }

        public string FormattedAddress(AatfReturn.AddressData address, bool includeSiteName = true)
        {
            if (address == null)
            {
                return string.Empty;
            }

            var siteAddressLong = includeSiteName ? address.Name + "<br/>" + address.Address1 : address.Address1;

            if (address.Address2 != null)
            {
                siteAddressLong += "<br/>" + address.Address2;
            }

            siteAddressLong += "<br/>" + address.TownOrCity;

            if (address.CountyOrRegion != null)
            {
                siteAddressLong += "<br/>" + address.CountyOrRegion;
            }

            if (address.Postcode != null)
            {
                siteAddressLong += "<br/>" + address.Postcode;
            }

            siteAddressLong += "<br/>" + address.CountryName;

            return siteAddressLong;
        }

        public string FormattedAddress(string name,
            string address1,
            string address2,
            string town,
            string county,
            string postCode,
            string approvalNumber = null)
        {
            var addressStringBuilder = new StringBuilder();
            const string spanTagName = "span";

            var address1Span = new TagBuilder(spanTagName);

            var nameSpan = new TagBuilder(spanTagName);
            nameSpan.SetInnerText($"{name}");
            addressStringBuilder.Append(nameSpan);

            if (approvalNumber != null)
            {
                var approvalNumberSpan = new TagBuilder(spanTagName);
                approvalNumberSpan.SetInnerText(approvalNumber);
                addressStringBuilder.Append($"<strong>{approvalNumberSpan}</strong>");
            }

            address1Span.SetInnerText($"{address1}");
            addressStringBuilder.Append(address1Span);

            if (address2 != null)
            {
                var address2Span = new TagBuilder(spanTagName);
                address2Span.SetInnerText($"{address2}");

                addressStringBuilder.Append(address2Span);
            }

            var townOrCitySpan = new TagBuilder(spanTagName);
            townOrCitySpan.SetInnerText($"{town}");
            addressStringBuilder.Append(townOrCitySpan);

            if (county != null)
            {
                var countySpan = new TagBuilder(spanTagName);
                countySpan.SetInnerText($"{county}");
                addressStringBuilder.Append(countySpan);
            }

            if (postCode != null)
            {
                var postCodeSpan = new TagBuilder(spanTagName);
                postCodeSpan.SetInnerText($"{postCode}");
                addressStringBuilder.Append(postCodeSpan);
            }

            return addressStringBuilder.ToString();
        }

        public string FormattedCompanyPcsAddress(string companyName,
            string name,
            string address1,
            string address2,
            string town,
            string county,
            string postCode,
            string approvalNumber = null)
        {
            if (AreStringsEqual(companyName, name))
            {
                return FormattedAddress(name, address1, address2, town, county, postCode, approvalNumber);
            }
            else
            {
                var stringBuilder = new StringBuilder();
                const string spanTagName = "span";
                var companyNameSpan = new TagBuilder(spanTagName);
                companyNameSpan.SetInnerText($"{companyName}");
                stringBuilder.Append(companyNameSpan);

                stringBuilder.Append(FormattedAddress(name, address1, address2, town, county, postCode, approvalNumber));
                return stringBuilder.ToString();
            }
        }

        public string FormattedApprovedRecipientDetails(string approvedRecipientDetails)
        {
            if (!string.IsNullOrEmpty(approvedRecipientDetails))
            {
                if (approvedRecipientDetails.Length > 0 && approvedRecipientDetails.Contains("<span>"))
                {
                    return approvedRecipientDetails;
                }

                var recipientDetails = approvedRecipientDetails.Split(new string[] { "<br/>" }, StringSplitOptions.None);
                var stringBuilder = new StringBuilder();
                const string spanTag = "span";

                if (recipientDetails.Length > 0)
                {
                    for (int count = 0; count < recipientDetails.Count(); count++)
                    {
                        var span = new TagBuilder(spanTag);
                        span.SetInnerText($"{recipientDetails[count].Trim()}");
                        stringBuilder.Append(span);
                    }
                }
                else
                {
                    var span = new TagBuilder(spanTag);
                    span.SetInnerText($"{approvedRecipientDetails}");
                    stringBuilder.Append(span);
                }

                return stringBuilder.ToString();
            }

            return string.Empty;
        }

        private bool AreStringsEqual(string s1, string s2)
        {
            var regex = @"[^aA-zZ]|[\^\[\]_\\`]";

            var compareString1 = Regex.Replace(s1, regex, string.Empty);
            var compareString2 = Regex.Replace(s2, regex, string.Empty);

            return compareString1.Equals(compareString2, StringComparison.OrdinalIgnoreCase);
        }
    }
}