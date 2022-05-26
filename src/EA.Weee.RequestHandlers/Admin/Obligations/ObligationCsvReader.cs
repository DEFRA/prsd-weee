namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System.Threading.Tasks;
    using Core.Shared;
    using Core.Shared.CsvReading;

    public class ObligationCsvReader : IObligationCsvReader
    {
        private readonly IFileHelper fileHelper;

        public ObligationCsvReader(IFileHelper fileHelper)
        {
            this.fileHelper = fileHelper;
        }

        public void ValidateHeader(byte[] data)
        {
            using (var reader = fileHelper.GetCsvReader(fileHelper.GetStreamReader(data)))
            {
                reader.RegisterClassMap<ObligationUploadClassMap>();
                reader.ReadHeader();
                reader.ValidateHeader<ObligationUpload>();
            }
        }
    }
}
