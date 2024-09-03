namespace EA.Weee.Api.Client
{
    using System;
    using System.Threading.Tasks;
    using EA.Weee.Api.Client.Entities.AddressLookup;

    public interface IAddressLookupClient : IDisposable
    {
        Task<AddressLookupResponse> GetAddressLookup(string endpoint, string companyReference);
    }
}