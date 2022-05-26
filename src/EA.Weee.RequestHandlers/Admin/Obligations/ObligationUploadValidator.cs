namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using Core.Shared.CsvReading;
    using Domain.Obligation;

    public class ObligationUploadValidator : IObligationUploadValidator
    {
        public IList<ObligationUploadError> Validate(IList<ObligationCsvUpload> obligations)
        {
            return new List<ObligationUploadError>();
        }
    }
}
