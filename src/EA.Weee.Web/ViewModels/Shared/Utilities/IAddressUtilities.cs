namespace EA.Weee.Web.ViewModels.Shared.Utilities
{
    using EA.Weee.Core.AatfReturn;

    public interface IAddressUtilities
    {
        string AddressConcatenate(AddressData addressData);
        string StringConcatenate(string address, string input);

        string FormattedAddress(AddressData address, bool includeSiteName = true);
    }
}