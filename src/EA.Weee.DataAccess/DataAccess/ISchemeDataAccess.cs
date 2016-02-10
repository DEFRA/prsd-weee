namespace EA.Weee.DataAccess.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Scheme;

    public interface ISchemeDataAccess
    {
        Task<Scheme> GetSchemeOrDefault(Guid schemeId);

        Task<IList<int>> GetComplianceYearsWithSubmittedMemberUploads(Guid schemeId);

        Task<IList<int>> GetComplianceYearsWithSubmittedDataReturns(Guid schemeId);
    }
}
