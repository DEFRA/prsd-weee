namespace EA.Weee.RequestHandlers.Admin.Reports.GetProducerEeeDataCsv
{
    using DataAccess.StoredProcedure;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Scheme = Domain.Scheme.Scheme;

    public interface IGetProducerEeeDataCsvDataAccess
    {
        Task<Scheme> GetSchemeAsync(Guid schemeId);

        Task<List<ProducerEeeCsvData>> GetItemsAsync(int complianceYear, Guid? schemeId, string obligationType);
    }
}
