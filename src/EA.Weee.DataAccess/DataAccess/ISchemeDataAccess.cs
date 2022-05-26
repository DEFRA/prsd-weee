namespace EA.Weee.DataAccess.DataAccess
{
    using Domain.Scheme;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ISchemeDataAccess
    {
        Task<Scheme> GetSchemeOrDefault(Guid schemeId);

        Task<Scheme> GetSchemeOrDefaultByApprovalNumber(string approvalNumber);
        Task<IList<int>> GetComplianceYearsWithSubmittedMemberUploads(Guid schemeId);

        Task<IList<int>> GetComplianceYearsWithSubmittedDataReturns(Guid schemeId);

        Task<Scheme> GetSchemeOrDefaultByOrganisationId(Guid organisationId);

        Task<List<string>> GetMemberRegistrationSchemesByComplianceYear(int complianceYear);

        Task<List<string>> GetEEEWEEEDataReturnSchemesByComplianceYear(int complianceYear);
    }
}
