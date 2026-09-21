namespace EA.Weee.RequestHandlers.Admin.GetSchemesExceedingRetentionPeriod
{
    using DataAccess.StoredProcedure;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Scheme = Domain.Scheme.Scheme;

    public interface IGetSchemeComplianceYearsExceedingRetentionPeriodDataAccess
    {
        Task<List<int>> GetItemsAsync();
    }
}
