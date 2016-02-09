namespace EA.Weee.RequestHandlers.Admin.Reports.GetProducerEeeDataCsv
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess.StoredProcedure;
    using Scheme = Domain.Scheme.Scheme;

    public interface IGetProducerEeeDataCsvDataAccess
    {
        Task<Scheme> GetSchemeAsync(Guid schemeId);

        Task<List<ProducerEeeCsvData>> GetItemsAsync(int complianceYear, Guid? schemeId, string obligationType);
    }
}
