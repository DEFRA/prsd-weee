namespace EA.Weee.Web.Areas.AatfReturn.Mappings.ToViewModel
{
    using EA.Weee.Core.AatfReturn;

    public interface IAddressUtilities
    {
        string AddressConcatenate(AddressData addressData);
        string StringConcatenate(string address, string input);
    }
}