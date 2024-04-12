namespace EA.Weee.Core.Shared
{
    public interface IAddressUtilities
    {
        string AddressConcatenate(AatfReturn.AddressData addressData);
        string StringConcatenate(string address, string input);
        string FormattedAddress(AatfReturn.AddressData address, bool includeSiteName = true);
        string FormattedAddress(string name, string address1, string address2, string town, string county,
            string postCode, string approvalNumber = null);
        string FormattedCompanyPcsAddress(string companyName, string name, string address1, string address2, string town, string county,
            string postCode, string approvalNumber = null);
        string FormattedApprovedRecipientDetails(string approvedRecipientDetails);
    }
}