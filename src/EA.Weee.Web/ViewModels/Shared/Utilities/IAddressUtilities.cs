namespace EA.Weee.Web.ViewModels.Shared.Utilities
{
    using EA.Weee.Core.AatfReturn;

    public interface IAddressUtilities
    {
        string AddressConcatenate(AddressData addressData);
        string StringConcatenate(string address, string input);
        string FormattedAddress(AddressData address, bool includeSiteName = true);
        string FormattedAddress(string name, string address1, string address2, string town, string county,
            string postCode, string approvalNumber = null);
    }
}