namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;

    public class AddressUtilities : IAddressUtilities
    {
        public string AddressConcatenate(AddressData addressData)
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

        public string FormattedAddress(AddressData address)
        {
            if (address == null)
            {
                return string.Empty;
            }

            var siteAddressLong = address.Name + "<br/>" + address.Address1;

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
    }
}