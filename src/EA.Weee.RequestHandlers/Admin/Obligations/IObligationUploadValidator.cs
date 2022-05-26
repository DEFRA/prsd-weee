namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using Domain.Obligation;
    using ObligationCsvUpload = Core.Shared.CsvReading.ObligationCsvUpload;

    public interface IObligationUploadValidator
    {
        IList<ObligationUploadError> Validate(IList<ObligationCsvUpload> obligations);
    }
}
