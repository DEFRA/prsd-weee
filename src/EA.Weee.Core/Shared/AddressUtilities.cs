namespace EA.Weee.Core.Shared
{
    using System;
    using System.Text;
    using System.Text.RegularExpressions;

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
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(name);

            if (approvalNumber != null)
            {
                stringBuilder.Append($"<br/><strong>{approvalNumber}</strong>");
            }

            stringBuilder.Append($"<br/>{address1}");

            if (address2 != null)
            {
                stringBuilder.Append($"<br/>{address2}");
            }

            stringBuilder.Append($"<br/>{town}");

            if (county != null)
            {
                stringBuilder.Append($"<br/>{county}");
            }

            if (postCode != null)
            {
                stringBuilder.Append($"<br/>{postCode}");
            }

            return stringBuilder.ToString();
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
                stringBuilder.Append($"{companyName}<br/>");
                stringBuilder.Append(FormattedAddress(name, address1, address2, town, county, postCode, approvalNumber));
                return stringBuilder.ToString();
            }
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