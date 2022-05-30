namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Obligation;
    using ObligationCsvUpload = Core.Shared.CsvReading.ObligationCsvUpload;

    public interface IObligationUploadValidator
    {
        Task<IList<ObligationUploadError>> Validate(IList<ObligationCsvUpload> obligations);
    }
}
