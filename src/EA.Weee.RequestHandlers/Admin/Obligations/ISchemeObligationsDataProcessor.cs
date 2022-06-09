namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Shared.CsvReading;
    using Domain.Obligation;

    public interface ISchemeObligationsDataProcessor
    {
        Task<List<ObligationScheme>> Build(List<ObligationCsvUpload> obligationCsvUploads, int complianceYear);
    }
}