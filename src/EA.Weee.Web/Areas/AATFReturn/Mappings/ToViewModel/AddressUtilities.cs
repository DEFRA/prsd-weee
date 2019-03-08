namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;

    public class AddressUtilities
    {
        public string AddressConcatenate(AddressData addressData)
        {
            var address = string.Empty;

            address = addressData.Address1;

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
    }
}