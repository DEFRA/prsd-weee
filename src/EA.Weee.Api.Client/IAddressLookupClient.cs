namespace EA.Weee.Api.Client
{
    using EA.Weee.Api.Client.Models.AddressLookup;
    using System;
    using System.Threading.Tasks;

    public interface IAddressLookupClient : IDisposable
    {
        Task<AddressLookupResponse> GetAddressDetailsAsync(string endpoint, string postcode, string street);

        Task<AddressLookupResponse> GetAddressesAsync(string endpoint, string postcode);
    }
}