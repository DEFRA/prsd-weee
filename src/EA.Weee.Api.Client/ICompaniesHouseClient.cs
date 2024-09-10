namespace EA.Weee.Api.Client
{
    using EA.Weee.Api.Client.Models;
    using System;
    using System.Threading.Tasks;

    public interface ICompaniesHouseClient : IDisposable
    {
        Task<DefraCompaniesHouseApiModel> GetCompanyDetailsAsync(string endpoint, string companyReference);
    }
}