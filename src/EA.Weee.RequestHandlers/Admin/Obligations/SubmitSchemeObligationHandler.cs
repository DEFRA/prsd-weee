namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using Core.Shared;
    using Core.Shared.CsvReading;
    using CsvHelper;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;

    internal class SubmitSchemeObligationHandler : IRequestHandler<SubmitSchemeObligation, object>
    {
        private readonly IWeeeCsvReader csvReader;
        private readonly IFileHelper fileHelper;

        public SubmitSchemeObligationHandler(IWeeeCsvReader csvReader, IFileHelper fileHelper)
        {
            this.csvReader = csvReader;
            this.fileHelper = fileHelper;
        }

        public Task<object> HandleAsync(SubmitSchemeObligation message)
        {
            using (var reader = fileHelper.GetCsvReader(fileHelper.GetStreamReader(message.FileInfo.Data)))
            {
                csvReader.RegisterClassMap<ObligationUploadClassMap>();

                try
                {
                    csvReader.ReadHeader();
                    csvReader.ValidateHeader<ObligationUpload>();
                }
                catch (CsvValidationException)
                {
                    throw new Exception();
                }
                catch (CsvReaderException)
                {
                    throw new Exception();
                }
            }

            return null;
        }
    }
}
