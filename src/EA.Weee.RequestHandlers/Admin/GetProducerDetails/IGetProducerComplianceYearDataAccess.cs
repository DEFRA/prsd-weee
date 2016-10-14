namespace EA.Weee.RequestHandlers.Admin.GetProducerDetails
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetProducerComplianceYearDataAccess
    {
        Task<List<int>> GetComplianceYears(string registrationNumber);
    }
}
