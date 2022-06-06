namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Core.Shared;
    using Core.Shared.CsvReading;
    using ObligationCsvUpload = Core.Shared.CsvReading.ObligationCsvUpload;

    public class ObligationCsvReader : IObligationCsvReader
    {
        private readonly IFileHelper fileHelper;
        private const int NumberOfColumns = 16;

        private readonly string[] columnOrder = new[]
        {
            "Scheme Identifier",
            "Scheme Name", "Cat1 (t)", "Cat2 (t)", "Cat3 (t)", "Cat4 (t)", "Cat5 (t)", "Cat6 (t)", "Cat7 (t)", "Cat8 (t)",
            "Cat9 (t)", "Cat10 (t)", "Cat11 (t)", "Cat12 (t)", "Cat13 (t)", "Cat14 (t)"
        };

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
                reader.ValidateHeader<ObligationCsvUpload>(NumberOfColumns, columnOrder);

                while (reader.Read())
                {
                    var obligation = reader.GetRecord<ObligationCsvUpload>();

                    obligations.Add(obligation);
                }
            }

            return obligations;
        }
    }
}
