namespace EA.Weee.Api.Client
{
    using System;
    using System.Threading.Tasks;

    public interface ICompaniesHouseClient : IDisposable
    {
        Task<T> GetCompanyDetailsAsync<T>(string endpoint, string companyReference);
    }
}