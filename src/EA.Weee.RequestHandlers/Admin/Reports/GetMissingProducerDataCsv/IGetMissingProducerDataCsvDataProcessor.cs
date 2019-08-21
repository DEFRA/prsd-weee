namespace EA.Weee.RequestHandlers.Admin.Reports.GetMissingProducerDataCsv
{
    using DataAccess.StoredProcedure;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IGetMissingProducerDataCsvDataProcessor
    {
        Task<List<MissingProducerDataCsvData>> FetchMissingProducerDataAsync(int complianceYear,
            string obligationType,
            int? quarter,
            Guid? schemeId);
    }
}
