namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using Core.Shared.CsvReading;

    public interface IObligationCsvReader
    {
        IList<ObligationCsvUpload> Read(byte[] data);
    }
}
