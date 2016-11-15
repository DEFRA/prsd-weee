namespace EA.Weee.RequestHandlers.Admin.Reports.GetMissingProducerDataCsv
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DataAccess.StoredProcedure;

    public interface IGetMissingProducerDataCsvDataProcessor
    {
        Task<List<MissingProducerDataCsvData>> FetchMissingProducerDataAsync(int complianceYear, 
            string obligationType,
            int? quarter,
            Guid? schemeId);
    }
}
