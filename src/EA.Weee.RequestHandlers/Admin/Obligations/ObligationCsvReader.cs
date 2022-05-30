namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Collections.Generic;
    using Core.Shared;
    using Core.Shared.CsvReading;
    using ObligationCsvUpload = Core.Shared.CsvReading.ObligationCsvUpload;

    public class ObligationCsvReader : IObligationCsvReader
    {
        private readonly IFileHelper fileHelper;

        public ObligationCsvReader(IFileHelper fileHelper)
        {
            this.fileHelper = fileHelper;
        }

        public IList<ObligationCsvUpload> Read(byte[] data)
        {
            var obligations = new List<ObligationCsvUpload>();

            using (var reader = fileHelper.GetCsvReader(fileHelper.GetStreamReader(data)))
            {
                reader.RegisterClassMap<ObligationUploadClassMap>();
                reader.ReadHeader();
                reader.ValidateHeader<ObligationCsvUpload>();

                while (reader.Read())
                {
                    obligations.Add(reader.GetRecord<ObligationCsvUpload>());
                }
            }

            return obligations;
        }
    }
}
