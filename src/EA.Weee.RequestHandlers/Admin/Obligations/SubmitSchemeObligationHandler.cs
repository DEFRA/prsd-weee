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
    using Domain.Security;
    using Prsd.Core.Mediator;
    using Requests.Admin.Obligations;
    using Security;
    using Weee.Security;
    using ObligationCsvUpload = Core.Shared.CsvReading.ObligationCsvUpload;

    internal class SubmitSchemeObligationHandler : IRequestHandler<SubmitSchemeObligation, Guid>
    {
        private const string FileFormatError =
            "The error may be a problem with the file structure, which prevents our system from validating your file. You should rectify this error before we can continue our validation process.";

        private readonly IObligationCsvReader obligationCsvReader;
        private readonly IObligationUploadValidator obligationUploadValidator;
        private readonly IWeeeAuthorization authorization;

        public SubmitSchemeObligationHandler(IObligationCsvReader obligationCsvReader, 
            IObligationUploadValidator obligationUploadValidator, 
            IWeeeAuthorization authorization)
        {
            this.obligationCsvReader = obligationCsvReader;
            this.obligationUploadValidator = obligationUploadValidator;
            this.authorization = authorization;
        }

        public async Task<Guid> HandleAsync(SubmitSchemeObligation request)
        {
            authorization.EnsureCanAccessInternalArea();
            authorization.EnsureUserInRole(Roles.InternalAdmin);

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

            var dataErrors = await obligationUploadValidator.Validate(obligations);

            errors.AddRange(dataErrors);
            
            return Guid.NewGuid();
        }
    }
}
