namespace EA.Weee.RequestHandlers.Admin.Obligations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Shared;
    using Core.Shared.CsvReading;
    using Domain.Error;
    using Domain.Obligation;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using ObligationCsvUpload = Core.Shared.CsvReading.ObligationCsvUpload;

    internal class SubmitSchemeObligationHandler : IRequestHandler<SubmitSchemeObligation, Guid>
    {
        private const string FileFormatError =
            "The error may be a problem with the file structure, which prevents our system from validating your file. You should rectify this error before we can continue our validation process.";

        private readonly IFileHelper fileHelper;
        private readonly IObligationCsvReader obligationCsvReader;

        public SubmitSchemeObligationHandler(IFileHelper fileHelper, 
            IObligationCsvReader obligationCsvReader)
        {
            this.fileHelper = fileHelper;
            this.obligationCsvReader = obligationCsvReader;
        }

        public Task<Guid> HandleAsync(SubmitSchemeObligation request)
        {
            var errors = new List<ObligationUploadError>();
            var obligations = new List<ObligationCsvUpload>();

            try
            {
                obligations = obligationCsvReader.Read(request.FileInfo.Data).ToList();
            }
            catch (CsvValidationException)
            {
                errors.Add(new ObligationUploadError(ObligationUploadErrorType.File, FileFormatError));
            }
            catch (CsvReaderException)
            {
                errors.Add(new ObligationUploadError(ObligationUploadErrorType.File, FileFormatError));
            }

            return Task.FromResult(Guid.NewGuid());
        }
    }
}
